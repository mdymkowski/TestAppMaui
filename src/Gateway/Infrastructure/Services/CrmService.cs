using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TestAppMaui.Gateway.Application.Common.Interfaces;
using TestAppMaui.SharedDDD.Contracts;
using TestAppMaui.Gateway.Infrastructure.Options;

namespace TestAppMaui.Gateway.Infrastructure.Services;

public sealed class CrmService : ICrmService
{
    private static readonly HttpMethod PatchMethod = new("PATCH");

    private readonly HttpClient _httpClient;
    private readonly CrmOptions _options;

    public CrmService(HttpClient httpClient, IOptions<CrmOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<TaskDto?> GetTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        var response = await _httpClient.GetAsync(BuildEntityUrl(id), cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        return ParseTask(document.RootElement, id);
    }

    public async Task<TaskDto> UpsertTaskAsync(TaskDto task, CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        var payload = CreatePayload(task);
        HttpResponseMessage response;

        if (task.Id == Guid.Empty)
        {
            response = await _httpClient.PostAsync(_options.TaskEntitySetName, JsonContent.Create(payload), cancellationToken);
        }
        else
        {
            var request = new HttpRequestMessage(PatchMethod, BuildEntityUrl(task.Id))
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.IfMatch.Add(EntityTagHeaderValue.Any);
            response = await _httpClient.SendAsync(request, cancellationToken);
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            var entityId = task.Id == Guid.Empty
                ? await ResolveEntityIdAsync(response, cancellationToken)
                : task.Id;

            var refreshed = await GetTaskAsync(entityId, cancellationToken);
            return refreshed ?? new TaskDto(entityId, task.Title, task.Description, task.DueDate, task.IsCompleted);
        }

        response.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength ?? 0;
        if (contentLength == 0)
        {
            var entityId = task.Id == Guid.Empty
                ? await ResolveEntityIdAsync(response, cancellationToken)
                : task.Id;

            var refreshed = await GetTaskAsync(entityId, cancellationToken);
            return refreshed ?? new TaskDto(entityId, task.Title, task.Description, task.DueDate, task.IsCompleted);
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            var entityId = task.Id == Guid.Empty
                ? await ResolveEntityIdAsync(response, cancellationToken)
                : task.Id;

            var refreshed = await GetTaskAsync(entityId, cancellationToken);
            return refreshed ?? new TaskDto(entityId, task.Title, task.Description, task.DueDate, task.IsCompleted);
        }

        using var document = JsonDocument.Parse(content);
        return ParseTask(document.RootElement, task.Id);
    }

    private string BuildEntityUrl(Guid id)
    {
        var selectedColumns = new List<string>();

        if (!string.IsNullOrWhiteSpace(_options.IdAttribute))
        {
            selectedColumns.Add(_options.IdAttribute);
        }

        if (!string.IsNullOrWhiteSpace(_options.TitleAttribute))
        {
            selectedColumns.Add(_options.TitleAttribute);
        }

        if (!string.IsNullOrWhiteSpace(_options.DescriptionAttribute))
        {
            selectedColumns.Add(_options.DescriptionAttribute);
        }

        if (!string.IsNullOrWhiteSpace(_options.DueDateAttribute))
        {
            selectedColumns.Add(_options.DueDateAttribute);
        }

        if (!string.IsNullOrWhiteSpace(_options.StateAttribute))
        {
            selectedColumns.Add(_options.StateAttribute);
        }

        var selectClause = selectedColumns.Count > 0
            ? $"?$select={string.Join(',', selectedColumns)}"
            : string.Empty;

        return $"{_options.TaskEntitySetName}({id}){selectClause}";
    }

    private Dictionary<string, object?> CreatePayload(TaskDto task)
    {
        var payload = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(_options.TitleAttribute))
        {
            payload[_options.TitleAttribute] = task.Title;
        }

        if (!string.IsNullOrWhiteSpace(_options.StateAttribute))
        {
            payload[_options.StateAttribute] = task.IsCompleted ? _options.CompletedStateValue : _options.ActiveStateValue;
        }

        if (!string.IsNullOrWhiteSpace(_options.DescriptionAttribute))
        {
            payload[_options.DescriptionAttribute] = task.Description;
        }

        if (!string.IsNullOrWhiteSpace(_options.DueDateAttribute))
        {
            payload[_options.DueDateAttribute] = task.DueDate?.ToUniversalTime().ToString("o");
        }

        return payload;
    }

    private TaskDto ParseTask(JsonElement element, Guid fallbackId)
    {
        var id = fallbackId;

        if (!string.IsNullOrWhiteSpace(_options.IdAttribute) &&
            element.TryGetProperty(_options.IdAttribute, out var idElement))
        {
            if (idElement.ValueKind == JsonValueKind.String && Guid.TryParse(idElement.GetString(), out var parsedId))
            {
                id = parsedId;
            }
        }

        var title = element.TryGetProperty(_options.TitleAttribute, out var titleElement) &&
                    titleElement.ValueKind != JsonValueKind.Null
            ? titleElement.GetString() ?? string.Empty
            : string.Empty;

        string? description = null;
        if (!string.IsNullOrWhiteSpace(_options.DescriptionAttribute) &&
            element.TryGetProperty(_options.DescriptionAttribute, out var descriptionElement) &&
            descriptionElement.ValueKind != JsonValueKind.Null)
        {
            description = descriptionElement.GetString();
        }

        DateTime? dueDate = null;
        if (!string.IsNullOrWhiteSpace(_options.DueDateAttribute) &&
            element.TryGetProperty(_options.DueDateAttribute, out var dueDateElement) &&
            dueDateElement.ValueKind == JsonValueKind.String &&
            DateTime.TryParse(dueDateElement.GetString(), out var parsedDue))
        {
            dueDate = parsedDue;
        }

        var isCompleted = false;
        if (!string.IsNullOrWhiteSpace(_options.StateAttribute) &&
            element.TryGetProperty(_options.StateAttribute, out var stateElement))
        {
            switch (stateElement.ValueKind)
            {
                case JsonValueKind.Number:
                    isCompleted = stateElement.GetInt32() == _options.CompletedStateValue;
                    break;
                case JsonValueKind.String when int.TryParse(stateElement.GetString(), out var state):
                    isCompleted = state == _options.CompletedStateValue;
                    break;
            }
        }

        return new TaskDto(id, title, description, dueDate, isCompleted);
    }

    private async Task<Guid> ResolveEntityIdAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Headers.TryGetValues("OData-EntityId", out var values))
        {
            var entityId = values.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(entityId))
            {
                var startIndex = entityId.IndexOf('(');
                var endIndex = entityId.IndexOf(')', startIndex + 1);
                if (startIndex >= 0 && endIndex > startIndex)
                {
                    var identifier = entityId.Substring(startIndex + 1, endIndex - startIndex - 1);
                    if (Guid.TryParse(identifier, out var parsed))
                    {
                        return parsed;
                    }
                }
            }
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(content))
        {
            using var document = JsonDocument.Parse(content);
            if (!string.IsNullOrWhiteSpace(_options.IdAttribute) &&
                document.RootElement.TryGetProperty(_options.IdAttribute, out var idElement) &&
                idElement.ValueKind == JsonValueKind.String &&
                Guid.TryParse(idElement.GetString(), out var parsedId))
            {
                return parsedId;
            }
        }

        throw new InvalidOperationException("Unable to determine CRM entity identifier from the response.");
    }

    private void EnsureConfigured()
    {
        if (!_options.IsConfigured)
        {
            throw new InvalidOperationException("CRM integration has not been configured.");
        }
    }
}
