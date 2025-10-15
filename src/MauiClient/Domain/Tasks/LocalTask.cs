namespace TestAppMaui.MauiClient.Domain.Tasks;

public sealed class LocalTask
{
    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime? DueDate { get; private set; }

    public bool IsCompleted { get; private set; }

    private LocalTask()
    {
    }

    private LocalTask(Guid id, string title, string? description, DateTime? dueDate, bool isCompleted)
    {
        Id = id;
        Title = title;
        Description = description;
        DueDate = dueDate;
        IsCompleted = isCompleted;
    }

    public static LocalTask Create(string title, string? description, DateTime? dueDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        return new LocalTask(Guid.NewGuid(), title, description, dueDate, false);
    }

    public static LocalTask FromExisting(Guid id, string title, string? description, DateTime? dueDate, bool isCompleted)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        return new LocalTask(id, title, description, dueDate, isCompleted);
    }

    public void UpdateDetails(string title, string? description, DateTime? dueDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Title = title;
        Description = description;
        DueDate = dueDate;
    }

    public void MarkComplete() => IsCompleted = true;

    public void MarkIncomplete() => IsCompleted = false;
}
