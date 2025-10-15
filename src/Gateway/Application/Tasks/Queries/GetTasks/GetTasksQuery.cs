using MediatR;
using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.Gateway.Application.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery : IRequest<IReadOnlyCollection<TaskDto>>;
