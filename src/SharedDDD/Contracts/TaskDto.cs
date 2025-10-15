namespace TestAppMaui.SharedDDD.Contracts;

public sealed record TaskDto(Guid Id, string Title, string? Description, DateTime? DueDate, bool IsCompleted);
