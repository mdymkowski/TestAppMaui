using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using TestAppMaui.MauiClient.Models;

namespace TestAppMaui.MauiClient.Services;

public sealed class SqliteLocalTaskStore : ILocalTaskStore, IAsyncDisposable
{
    private const string CreateTableSql = @"CREATE TABLE IF NOT EXISTS Tasks (
        Id TEXT PRIMARY KEY NOT NULL,
        Title TEXT NOT NULL,
        Description TEXT NULL,
        DueDate TEXT NULL,
        IsCompleted INTEGER NOT NULL
    );";

    private const string SelectSql = @"SELECT Id, Title, Description, DueDate, IsCompleted FROM Tasks ORDER BY DueDate IS NULL, DueDate";

    private const string DeleteAllSql = @"DELETE FROM Tasks";

    private const string UpsertSql = @"INSERT INTO Tasks (Id, Title, Description, DueDate, IsCompleted)
VALUES (@Id, @Title, @Description, @DueDate, @IsCompleted)
ON CONFLICT(Id) DO UPDATE SET Title = excluded.Title, Description = excluded.Description, DueDate = excluded.DueDate, IsCompleted = excluded.IsCompleted";

    private readonly string _connectionString;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);

    private bool _initialized;

    public SqliteLocalTaskStore(string databasePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(databasePath);

        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _connectionString = $"Data Source={databasePath};Cache=Shared";
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TaskDto>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken).ConfigureAwait(false);

        await using var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var command = connection.CreateCommand();
        command.CommandText = SelectSql;

        var tasks = new List<TaskDto>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var id = Guid.Parse(reader.GetString(0));
            var title = reader.GetString(1);
            var description = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateTime? dueDate = null;
            if (!reader.IsDBNull(3))
            {
                var dueDateString = reader.GetString(3);
                if (DateTime.TryParse(dueDateString, out var parsedDueDate))
                {
                    dueDate = parsedDueDate;
                }
            }

            var isCompleted = reader.GetInt32(4) == 1;
            tasks.Add(new TaskDto(id, title, description, dueDate, isCompleted));
        }

        return tasks;
    }

    public async Task ReplaceTasksAsync(IEnumerable<TaskDto> tasks, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        await EnsureInitializedAsync(cancellationToken).ConfigureAwait(false);

        await using var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        await using (var deleteCommand = connection.CreateCommand())
        {
            deleteCommand.CommandText = DeleteAllSql;
            deleteCommand.Transaction = transaction;
            await deleteCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        foreach (var task in tasks)
        {
            await UpsertAsync(connection, transaction, task, cancellationToken).ConfigureAwait(false);
        }

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddOrUpdateTaskAsync(TaskDto task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        await EnsureInitializedAsync(cancellationToken).ConfigureAwait(false);

        await using var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await UpsertAsync(connection, transaction: null, task, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        _initializationLock.Dispose();
        await Task.CompletedTask;
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        await _initializationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_initialized)
            {
                return;
            }

            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = CreateTableSql;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private static async Task UpsertAsync(SqliteConnection connection, SqliteTransaction? transaction, TaskDto task, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = UpsertSql;
        command.Transaction = transaction;

        command.Parameters.AddWithValue("@Id", task.Id.ToString());
        command.Parameters.AddWithValue("@Title", task.Title);
        command.Parameters.AddWithValue("@Description", (object?)task.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@DueDate", task.DueDate?.ToUniversalTime().ToString("o") ?? (object?)DBNull.Value);
        command.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
