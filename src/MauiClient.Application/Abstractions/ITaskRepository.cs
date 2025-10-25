using TestAppMaui.MauiClient.Domain.Tasks;

namespace TestAppMaui.MauiClient.Application.Abstractions;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
}
