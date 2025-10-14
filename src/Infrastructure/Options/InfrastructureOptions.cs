namespace TestAppMaui.Infrastructure.Options;

public sealed class InfrastructureOptions
{
    public DatabaseOptions Database { get; } = new();

    public CrmOptions Crm { get; } = new();

    public void UseSqlite(string databasePath)
    {
        Database.UseSqlite(databasePath);
    }

    public void UseSqlServer(string connectionString)
    {
        Database.UseSqlServer(connectionString);
    }

    public void ConfigureCrm(Action<CrmOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(Crm);
    }
}

public sealed class DatabaseOptions
{
    internal DatabaseProvider Provider { get; private set; } = DatabaseProvider.None;

    internal string? ConnectionString { get; private set; }

    internal string? SqliteDatabasePath { get; private set; }

    internal bool IsConfigured => Provider != DatabaseProvider.None;

    public void UseSqlite(string databasePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(databasePath);

        Provider = DatabaseProvider.Sqlite;
        SqliteDatabasePath = databasePath;
        ConnectionString = $"Data Source={databasePath}";
    }

    public void UseSqlServer(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        Provider = DatabaseProvider.SqlServer;
        ConnectionString = connectionString;
        SqliteDatabasePath = null;
    }
}

public enum DatabaseProvider
{
    None = 0,
    Sqlite = 1,
    SqlServer = 2
}
