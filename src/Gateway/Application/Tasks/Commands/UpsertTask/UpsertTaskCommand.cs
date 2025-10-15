using MediatR;
using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.Gateway.Application.Tasks.Commands.UpsertTask;

public sealed record UpsertTaskCommand(TaskDto Task) : IRequest;
