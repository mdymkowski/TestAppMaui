using Dawn;
using MediatR;
using TestAppMaui.Application.Common.Models;

namespace TestAppMaui.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(string Title, string? Description, DateTime? DueDate) : IRequest<TaskDto>
{
    public void Validate()
    {
        Guard.Argument(Title, nameof(Title)).NotNull().NotWhiteSpace();
    }
}
