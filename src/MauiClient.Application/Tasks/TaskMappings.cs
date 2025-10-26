using System.Collections.Generic;
using System.Linq;
using TestAppMaui.MauiClient.Domain.Tasks;

namespace TestAppMaui.MauiClient.Application.Tasks;

public static class TaskMappings
{
    public static TaskDto ToDto(this TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        return new TaskDto(task.Id, task.Name, task.Description);
    }

    public static IReadOnlyList<TaskDto> ToDto(this IEnumerable<TaskItem> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        return tasks.Select(task => task.ToDto()).ToList();
    }
}
