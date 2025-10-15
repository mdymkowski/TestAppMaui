using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.Gateway.Application.Common.Interfaces;

public interface ICrmService
{
    Task<TaskDto?> GetTaskAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TaskDto> UpsertTaskAsync(TaskDto task, CancellationToken cancellationToken = default);
}
