namespace TestAppMaui.Gateway.Api.Models;

internal sealed record CrmTaskRequest(string Title, string? Description, DateTime? DueDate, bool IsCompleted, Guid? Id);
