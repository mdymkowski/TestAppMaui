using System.Linq;
using Microsoft.EntityFrameworkCore;
using TestAppMaui.MauiClient.Application.Abstractions;
using TestAppMaui.MauiClient.Domain.Tasks;
using TestAppMaui.MauiClient.Infrastructure.Persistence;
using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.MauiClient.Infrastructure.Storage;

public sealed class EfCoreLocalTaskStore : ILocalTaskStore
{
    private readonly IDbContextFactory<LocalTaskDbContext> _contextFactory;

    public EfCoreLocalTaskStore(IDbContextFactory<LocalTaskDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TaskDto>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var tasks = await context.Tasks
            .AsNoTracking()
            .OrderBy(task => task.DueDate == null)
            .ThenBy(task => task.DueDate)
            .Select(task => new TaskDto(task.Id, task.Title, task.Description, task.DueDate, task.IsCompleted))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return tasks;
    }

    public async Task ReplaceTasksAsync(IEnumerable<TaskDto> tasks, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var existingTasks = await context.Tasks.ToListAsync(cancellationToken).ConfigureAwait(false);
        context.Tasks.RemoveRange(existingTasks);

        foreach (var task in tasks)
        {
            context.Tasks.Add(LocalTask.FromExisting(task.Id, task.Title, task.Description, task.DueDate, task.IsCompleted));
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddOrUpdateTaskAsync(TaskDto task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var existing = await context.Tasks.FindAsync(new object?[] { task.Id }, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            context.Tasks.Add(LocalTask.FromExisting(task.Id, task.Title, task.Description, task.DueDate, task.IsCompleted));
        }
        else
        {
            existing.UpdateDetails(task.Title, task.Description, task.DueDate);

            if (task.IsCompleted)
            {
                existing.MarkComplete();
            }
            else
            {
                existing.MarkIncomplete();
            }
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
