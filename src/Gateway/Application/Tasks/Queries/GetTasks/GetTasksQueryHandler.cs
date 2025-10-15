using MediatR;
using TestAppMaui.Gateway.Application.Common.Interfaces;
using TestAppMaui.SharedDDD.Contracts;
using TestAppMaui.Gateway.Domain.Aggregates.Tasks;

namespace TestAppMaui.Gateway.Application.Tasks.Queries.GetTasks;

public sealed class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IReadOnlyCollection<TaskDto>>
{
    private readonly IRepository<TaskItem> _repository;

    public GetTasksQueryHandler(IRepository<TaskItem> repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _repository.ListAsync(cancellationToken);
        return tasks
            .Select(task => new TaskDto(task.Id, task.Title, task.Description, task.DueDate, task.IsCompleted))
            .ToList();
    }
}
