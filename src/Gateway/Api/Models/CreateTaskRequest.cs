namespace TestAppMaui.Gateway.Api.Models;

internal sealed record CreateTaskRequest(string Title, string? Description, DateTime? DueDate);
