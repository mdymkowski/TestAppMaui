using MediatR;
using TestAppMaui.Application.Common.Models;

namespace TestAppMaui.Application.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery : IRequest<IReadOnlyCollection<TaskDto>>;
