using System;

namespace TestAppMaui.MauiClient.Domain.Tasks;

public sealed class TaskItem
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public const int DescriptionMaxLength = 250;

    private TaskItem()
    {
    }

    private TaskItem(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public static TaskItem Create(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalizedDescription = NormalizeDescription(description);
        return new TaskItem(Guid.NewGuid(), name, normalizedDescription);
    }

    public static TaskItem FromExisting(Guid id, string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var normalizedDescription = NormalizeDescription(description);
        return new TaskItem(id, name, normalizedDescription);
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > DescriptionMaxLength)
        {
            throw new ArgumentException($"Description cannot exceed {DescriptionMaxLength} characters.", nameof(description));
        }

        return trimmedDescription;
    }
}
