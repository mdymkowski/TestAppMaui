using MediatR;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Application.Common.Models;
using TestAppMaui.Domain.Aggregates.Tasks;

namespace TestAppMaui.Application.Tasks.Queries.GetTasks;

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
