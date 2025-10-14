using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Infrastructure.Data;
using TestAppMaui.Infrastructure.Repositories;
using TestAppMaui.Infrastructure.Services;

namespace TestAppMaui.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string sqliteDatabasePath)
    {
        var directory = Path.GetDirectoryName(sqliteDatabasePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={sqliteDatabasePath}"));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IAppDatabaseInitializer, SqliteAppDatabaseInitializer>();

        return services;
    }
}
