using System;

namespace TestAppMaui.MauiClient.Domain.Tasks;

public sealed class TaskItem
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    private TaskItem()
    {
    }

    private TaskItem(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static TaskItem Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new TaskItem(Guid.NewGuid(), name);
    }

    public static TaskItem FromExisting(Guid id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new TaskItem(id, name);
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}
