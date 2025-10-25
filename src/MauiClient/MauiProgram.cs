using System.IO;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using TestAppMaui.MauiClient.Application.Abstractions;
using TestAppMaui.MauiClient.Application.Behaviors;
using TestAppMaui.MauiClient.Application.Tasks.Commands.CreateTask;
using TestAppMaui.MauiClient.Infrastructure.Persistence;
using TestAppMaui.MauiClient.Infrastructure.Tasks;
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

        builder.Services
            .AddDbContextFactory<TaskDbContext>(options => options.UseSqlite($"Data Source={databasePath}"))
            .AddSingleton<ITaskRepository, EfCoreTaskRepository>()
            .AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly))
            .AddValidatorsFromAssembly(typeof(CreateTaskCommandValidator).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddSingleton<MainViewModel>()
            .AddSingleton<MainPage>();

        var app = builder.Build();

        return app;
    }
}
