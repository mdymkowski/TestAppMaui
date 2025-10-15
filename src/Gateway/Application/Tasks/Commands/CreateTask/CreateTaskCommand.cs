using Dawn;
using MediatR;
using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.Gateway.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(string Title, string? Description, DateTime? DueDate) : IRequest<TaskDto>
{
    public void Validate()
    {
        Guard.Argument(Title, nameof(Title)).NotNull().NotWhiteSpace();
    }
}
