using MediatR;
using Microsoft.OpenApi.Models;
using TestAppMaui.Application;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Application.Tasks.Commands.CreateTask;
using TestAppMaui.Application.Tasks.Queries.GetTasks;
using TestAppMaui.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TestAppMaui Gateway",
        Version = "v1"
    });
});

builder.Services.AddApplication();

var databasePath = Path.Combine(AppContext.BaseDirectory, "gateway.db");
builder.Services.AddInfrastructure(databasePath);

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/tasks", async (IMediator mediator, CancellationToken cancellationToken) =>
{
    var tasks = await mediator.Send(new GetTasksQuery(), cancellationToken);
    return Results.Ok(tasks);
});

app.MapPost("/tasks", async (CreateTaskRequest request, IMediator mediator, CancellationToken cancellationToken) =>
{
    var command = new CreateTaskCommand(request.Title, request.Description, request.DueDate);
    var created = await mediator.Send(command, cancellationToken);
    return Results.Created($"/tasks/{created.Id}", created);
});

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IAppDatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.Run();

internal sealed record CreateTaskRequest(string Title, string? Description, DateTime? DueDate);
