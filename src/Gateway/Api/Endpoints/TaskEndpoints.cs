using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MediatR;
using TestAppMaui.Gateway.Application.Tasks.Commands.CreateTask;
using TestAppMaui.Gateway.Application.Tasks.Queries.GetTasks;
using TestAppMaui.Gateway.Api.Models;

namespace TestAppMaui.Gateway.Api.Endpoints;

internal static class TaskEndpoints
{
    internal static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/tasks", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var tasks = await mediator.Send(new GetTasksQuery(), cancellationToken);
            return Results.Ok(tasks);
        });

        endpoints.MapPost("/tasks", async (CreateTaskRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var command = new CreateTaskCommand(request.Title, request.Description, request.DueDate);
            var created = await mediator.Send(command, cancellationToken);
            return Results.Created($"/tasks/{created.Id}", created);
        });

        return endpoints;
    }
}
