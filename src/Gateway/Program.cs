using MediatR;
using Microsoft.OpenApi.Models;
using TestAppMaui.Application;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Application.Common.Models;
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
var connectionString = builder.Configuration.GetConnectionString("GatewayDatabase")
    ?? throw new InvalidOperationException("Connection string 'GatewayDatabase' is not configured.");

var crmSection = builder.Configuration.GetSection("Crm");
var crmConfigured = !string.IsNullOrWhiteSpace(crmSection.GetValue<string>("BaseUrl"));

builder.Services.AddInfrastructure(options =>
{
    options.UseSqlServer(connectionString);

    if (crmConfigured)
    {
        options.ConfigureCrm(crm => crmSection.Bind(crm));
    }
});

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

if (crmConfigured)
{
    app.MapGet("/crm/tasks/{id:guid}", async (Guid id, ICrmService crmService, CancellationToken cancellationToken) =>
    {
        var task = await crmService.GetTaskAsync(id, cancellationToken);
        return task is null ? Results.NotFound() : Results.Ok(task);
    });

    app.MapPost("/crm/tasks", async (CrmTaskRequest request, ICrmService crmService, CancellationToken cancellationToken) =>
    {
        var dto = new TaskDto(request.Id ?? Guid.Empty, request.Title, request.Description, request.DueDate, request.IsCompleted);
        var result = await crmService.UpsertTaskAsync(dto, cancellationToken);
        return Results.Ok(result);
    });
}

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IAppDatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.Run();

internal sealed record CreateTaskRequest(string Title, string? Description, DateTime? DueDate);
internal sealed record CrmTaskRequest(string Title, string? Description, DateTime? DueDate, bool IsCompleted, Guid? Id);
