namespace TestAppMaui.MauiClient.Models;

public sealed record TaskDto(Guid Id, string Title, string? Description, DateTime? DueDate, bool IsCompleted);
