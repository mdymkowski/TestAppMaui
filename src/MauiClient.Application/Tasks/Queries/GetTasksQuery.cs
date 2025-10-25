using MediatR;
using TestAppMaui.MauiClient.Application.Tasks;

namespace TestAppMaui.MauiClient.Application.Tasks.Queries;

public sealed record GetTasksQuery() : IRequest<IReadOnlyList<TaskDto>>;
