using Dawn;
using TestAppMaui.Domain.Common;

namespace TestAppMaui.Domain.Aggregates.Tasks;

public sealed class TaskItem : Entity, IAggregateRoot
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime? DueDate { get; private set; }
    public bool IsCompleted { get; private set; }

    private TaskItem()
    {
        Title = string.Empty;
    }

    public TaskItem(string title, string? description, DateTime? dueDate)
    {
        Title = Guard.Argument(title, nameof(title)).NotNull().NotWhiteSpace().Value;
        Description = description;
        DueDate = dueDate;
    }

    public void UpdateDetails(string title, string? description, DateTime? dueDate)
    {
        Title = Guard.Argument(title, nameof(title)).NotNull().NotWhiteSpace().Value;
        Description = description;
        DueDate = dueDate;
    }

    public void MarkComplete()
    {
        IsCompleted = true;
    }

    public void MarkIncomplete()
    {
        IsCompleted = false;
    }
}
