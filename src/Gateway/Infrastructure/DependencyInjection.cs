using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TestAppMaui.Gateway.Application.Common.Interfaces;
using TestAppMaui.Gateway.Infrastructure.Data;
using TestAppMaui.Gateway.Infrastructure.Options;
using TestAppMaui.Gateway.Infrastructure.Repositories;
using TestAppMaui.Gateway.Infrastructure.Services;

namespace TestAppMaui.Gateway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGatewayInfrastructure(this IServiceCollection services, Action<InfrastructureOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new InfrastructureOptions();
        configure(options);

        if (!options.Database.IsConfigured)
        {
            throw new InvalidOperationException("The database provider must be configured for infrastructure services.");
        }

        services.AddDbContext<ApplicationDbContext>(builder =>
        {
            switch (options.Database.Provider)
            {
                case DatabaseProvider.Sqlite:
                    if (!string.IsNullOrWhiteSpace(options.Database.SqliteDatabasePath))
                    {
                        var directory = Path.GetDirectoryName(options.Database.SqliteDatabasePath);
                        if (!string.IsNullOrWhiteSpace(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                    }

                    builder.UseSqlite(options.Database.ConnectionString);
                    break;

                case DatabaseProvider.SqlServer:
                    builder.UseSqlServer(options.Database.ConnectionString);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported database provider: {options.Database.Provider}");
            }
        });

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IAppDatabaseInitializer, RelationalAppDatabaseInitializer>();

        if (options.Crm.IsConfigured)
        {
            services.AddOptions<CrmOptions>()
                .Configure(o => options.Crm.CopyTo(o));

            services.AddHttpClient<ICrmService, CrmService>()
                .ConfigureHttpClient((provider, client) =>
                {
                    var crmOptions = provider.GetRequiredService<IOptions<CrmOptions>>().Value;
                    crmOptions.ConfigureClient(client);
                })
                .ConfigurePrimaryHttpMessageHandler(provider =>
                {
                    var crmOptions = provider.GetRequiredService<IOptions<CrmOptions>>().Value;
                    return crmOptions.CreateHandler();
                });
        }

        return services;
    }
}
