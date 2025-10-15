using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TestAppMaui.Gateway.Api.Models;
using TestAppMaui.Gateway.Application.Common.Interfaces;
using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.Gateway.Api.Endpoints;

internal static class CrmTaskEndpoints
{
    internal static IEndpointRouteBuilder MapCrmTaskEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/crm/tasks/{id:guid}", async (Guid id, ICrmService crmService, CancellationToken cancellationToken) =>
        {
            var task = await crmService.GetTaskAsync(id, cancellationToken);
            return task is null ? Results.NotFound() : Results.Ok(task);
        });

        endpoints.MapPost("/crm/tasks", async (CrmTaskRequest request, ICrmService crmService, CancellationToken cancellationToken) =>
        {
            var dto = new TaskDto(request.Id ?? Guid.Empty, request.Title, request.Description, request.DueDate, request.IsCompleted);
            var result = await crmService.UpsertTaskAsync(dto, cancellationToken);
            return Results.Ok(result);
        });

        return endpoints;
    }
}
