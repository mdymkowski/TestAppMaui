using System.Collections.Generic;
using System.Linq;
using MediatR;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Application.Tasks.Commands.ReplaceTasks;
using TestAppMaui.Application.Tasks.Commands.UpsertTask;
using TestAppMaui.Application.Tasks.Queries.GetTasks;
using ApplicationTaskDto = TestAppMaui.Application.Common.Models.TaskDto;
using MauiTaskDto = TestAppMaui.MauiClient.Models.TaskDto;

namespace TestAppMaui.MauiClient.Services;

public sealed class ApplicationLocalTaskStore : ILocalTaskStore
{
    private readonly IAppDatabaseInitializer _databaseInitializer;
    private readonly IMediator _mediator;

    public ApplicationLocalTaskStore(IAppDatabaseInitializer databaseInitializer, IMediator mediator)
    {
        _databaseInitializer = databaseInitializer;
        _mediator = mediator;
    }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        return _databaseInitializer.InitializeAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MauiTaskDto>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _mediator.Send(new GetTasksQuery(), cancellationToken).ConfigureAwait(false);
        return tasks.Select(MapToMauiDto).ToList();
    }

    public Task ReplaceTasksAsync(IEnumerable<MauiTaskDto> tasks, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        var command = new ReplaceTasksCommand(tasks.Select(MapToApplicationDto).ToList());
        return _mediator.Send(command, cancellationToken);
    }

    public Task AddOrUpdateTaskAsync(MauiTaskDto task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        var command = new UpsertTaskCommand(MapToApplicationDto(task));
        return _mediator.Send(command, cancellationToken);
    }

    private static MauiTaskDto MapToMauiDto(ApplicationTaskDto dto)
    {
        return new MauiTaskDto(dto.Id, dto.Title, dto.Description, dto.DueDate, dto.IsCompleted);
    }

    private static ApplicationTaskDto MapToApplicationDto(MauiTaskDto dto)
    {
        return new ApplicationTaskDto(dto.Id, dto.Title, dto.Description, dto.DueDate, dto.IsCompleted);
    }
}
