using MediatR;
using TestAppMaui.MauiClient.Application.Abstractions;
using TestAppMaui.MauiClient.Application.Tasks;
using TestAppMaui.MauiClient.Domain.Tasks;

namespace TestAppMaui.MauiClient.Application.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;

    public CreateTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = TaskItem.Create(request.Name);
        await _taskRepository.AddAsync(task, cancellationToken).ConfigureAwait(false);
        return task.ToDto();
    }
}
