using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using TestAppMaui.Application;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Infrastructure;
using TestAppMaui.MauiClient.ViewModels;
using TestAppMaui.MauiClient.Views;

namespace TestAppMaui.MauiClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "testappmaui.db");

        builder.Services
            .AddSingleton<MainViewModel>()
            .AddSingleton<MainPage>();

        builder.Services
            .AddApplication()
            .AddInfrastructure(databasePath);

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IAppDatabaseInitializer>();
        initializer.InitializeAsync().GetAwaiter().GetResult();

        return app;
    }
}
