using MediatR;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Application.Common.Models;
using TestAppMaui.Domain.Aggregates.Tasks;

namespace TestAppMaui.Application.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IRepository<TaskItem> _repository;

    public CreateTaskCommandHandler(IRepository<TaskItem> repository)
    {
        _repository = repository;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        request.Validate();

        var task = new TaskItem(request.Title, request.Description, request.DueDate);
        await _repository.AddAsync(task, cancellationToken);

        return new TaskDto(task.Id, task.Title, task.Description, task.DueDate, task.IsCompleted);
    }
}
