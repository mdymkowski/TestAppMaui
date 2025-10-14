using TestAppMaui.Application.Common.Models;

namespace TestAppMaui.Application.Common.Interfaces;

public interface ICrmService
{
    Task<TaskDto?> GetTaskAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TaskDto> UpsertTaskAsync(TaskDto task, CancellationToken cancellationToken = default);
}
