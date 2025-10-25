namespace TestAppMaui.MauiClient.Domain;

public sealed record CreateTaskRequest(string Title, string? Description, DateTime? DueDate);
