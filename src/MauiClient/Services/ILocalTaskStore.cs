using TestAppMaui.MauiClient.Models;

namespace TestAppMaui.MauiClient.Services;

public interface ILocalTaskStore
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskDto>> GetTasksAsync(CancellationToken cancellationToken = default);

    Task ReplaceTasksAsync(IEnumerable<TaskDto> tasks, CancellationToken cancellationToken = default);

    Task AddOrUpdateTaskAsync(TaskDto task, CancellationToken cancellationToken = default);
}
