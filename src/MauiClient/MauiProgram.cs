using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using TestAppMaui.Infrastructure.Data;
using TestAppMaui.MauiClient.Services;
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
            .AddDbContextFactory<ApplicationDbContext>(options =>
            {
                options.UseSqlite($"Data Source={databasePath}");
            })
            .AddSingleton<MainViewModel>()
            .AddSingleton<MainPage>()
            .AddSingleton<ILocalTaskStore, EfCoreLocalTaskStore>()
            .AddHttpClient<IGatewayApiClient, GatewayApiClient>(client =>
            {
                client.BaseAddress = new Uri(gatewayBaseUrl);
            });

        var app = builder.Build();

        return app;
    }
}
