using Microsoft.EntityFrameworkCore;
using TestAppMaui.Gateway.Application.Common.Interfaces;
using TestAppMaui.Gateway.Infrastructure.Data;

namespace TestAppMaui.Gateway.Infrastructure.Services;

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
