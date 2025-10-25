using MediatR;
using TestAppMaui.MauiClient.Application.Abstractions;
using TestAppMaui.MauiClient.Application.Tasks;

namespace TestAppMaui.MauiClient.Application.Tasks.Queries;

public sealed class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return tasks.ToDto();
    }
}
