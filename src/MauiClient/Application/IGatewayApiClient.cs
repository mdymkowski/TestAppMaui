using TestAppMaui.MauiClient.Domain;
using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.MauiClient.Application.Abstractions;

public interface IGatewayApiClient
{
    Task<IReadOnlyList<TaskDto>> GetTasksAsync(CancellationToken cancellationToken = default);

    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
}
