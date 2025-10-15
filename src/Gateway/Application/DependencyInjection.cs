using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TestAppMaui.Gateway.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGatewayApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}
