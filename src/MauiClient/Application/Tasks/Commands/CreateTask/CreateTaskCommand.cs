using MediatR;
using TestAppMaui.MauiClient.Application.Tasks;

namespace TestAppMaui.MauiClient.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(string Name) : IRequest<TaskDto>;
