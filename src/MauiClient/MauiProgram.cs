using CommunityToolkit.Maui.Markup;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using TestAppMaui.MauiClient.Application.Abstractions;
using TestAppMaui.MauiClient.Application.Behaviors;
using TestAppMaui.MauiClient.Application.Tasks.Commands.CreateTask;
using TestAppMaui.MauiClient.Infrastructure.Persistence;
using TestAppMaui.MauiClient.Infrastructure.Tasks;
using TestAppMaui.MauiClient.ViewModels;
using TestAppMaui.Views;

namespace TestAppMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitMarkup();

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "testappmaui.db");
        var databaseDirectory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }

        builder.Services
            .AddDbContextFactory<TaskDbContext>(
                options =>
                    options.UseSqlite($"Data Source={databasePath}",
                    b => b.MigrationsAssembly("MauiClient.Infrastructure")))
            .AddSingleton<ITaskRepository, EfCoreTaskRepository>()
            .AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly))
            .AddValidatorsFromAssembly(typeof(CreateTaskCommandValidator).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddSingleton<AppShell>()
            .AddTransient<MainViewModel>()
            .AddTransient<MainPage>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TaskDbContext>>();
            using var db = factory.CreateDbContext();
            var pending = db.Database.GetPendingMigrations().ToList();
            System.Diagnostics.Debug.WriteLine("Pending migrations: " + string.Join(", ", pending));

            db.Database.Migrate();
            // lub: await db.Database.MigrateAsync();
        }

        return app;
    }
}