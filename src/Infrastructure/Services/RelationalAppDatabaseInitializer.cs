using Microsoft.EntityFrameworkCore;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Infrastructure.Data;

namespace TestAppMaui.Infrastructure.Services;

public sealed class RelationalAppDatabaseInitializer : IAppDatabaseInitializer
{
    private readonly ApplicationDbContext _dbContext;

    public RelationalAppDatabaseInitializer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);
    }
}
