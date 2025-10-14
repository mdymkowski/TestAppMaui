using TestAppMaui.MauiClient.Models;

namespace TestAppMaui.MauiClient.Services;

public interface IGatewayApiClient
{
    Task<IReadOnlyList<TaskDto>> GetTasksAsync(CancellationToken cancellationToken = default);

    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
}
