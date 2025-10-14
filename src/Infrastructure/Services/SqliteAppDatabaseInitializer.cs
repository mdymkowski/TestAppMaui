using Microsoft.EntityFrameworkCore;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Infrastructure.Data;

namespace TestAppMaui.Infrastructure.Services;

public sealed class SqliteAppDatabaseInitializer : IAppDatabaseInitializer
{
    private readonly ApplicationDbContext _dbContext;

    public SqliteAppDatabaseInitializer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}
