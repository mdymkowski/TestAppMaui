using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using TestAppMaui.MauiClient.Application.Abstractions;
using TestAppMaui.MauiClient.Infrastructure.Gateway;
using TestAppMaui.MauiClient.Infrastructure.Persistence;
using TestAppMaui.MauiClient.Infrastructure.Storage;
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
        var databaseDirectory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }

        var gatewayBaseUrl = Environment.GetEnvironmentVariable("GATEWAY_BASE_URL")
            ?? builder.Configuration["Gateway:BaseUrl"]
            ?? "https://localhost:5001/";

        if (!gatewayBaseUrl.EndsWith('/'))
        {
            gatewayBaseUrl += "/";
        }

        builder.Services
            .AddDbContextFactory<LocalTaskDbContext>(options => options.UseSqlite(databasePath))
            .AddSingleton<ILocalTaskStore, EfCoreLocalTaskStore>()
            .AddSingleton<MainViewModel>()
            .AddSingleton<MainPage>()
            .AddHttpClient<IGatewayApiClient, GatewayApiClient>(client =>
            {
                client.BaseAddress = new Uri(gatewayBaseUrl);
            });

        var app = builder.Build();

        return app;
    }
}
