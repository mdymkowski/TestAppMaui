using System.Net.Http.Json;
using System.Text.Json;
using TestAppMaui.MauiClient.Models;

namespace TestAppMaui.MauiClient.Services;

public sealed class GatewayApiClient : IGatewayApiClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public GatewayApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<TaskDto>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _httpClient.GetFromJsonAsync<IReadOnlyList<TaskDto>>("tasks", SerializerOptions, cancellationToken);
        return tasks ?? Array.Empty<TaskDto>();
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("tasks", request, SerializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var task = await response.Content.ReadFromJsonAsync<TaskDto>(SerializerOptions, cancellationToken);
        return task ?? throw new InvalidOperationException("Gateway API returned an empty response when creating a task.");
    }
}
