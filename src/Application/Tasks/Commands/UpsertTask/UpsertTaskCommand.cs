using MediatR;
using TestAppMaui.Application.Common.Models;

namespace TestAppMaui.Application.Tasks.Commands.UpsertTask;

public sealed record UpsertTaskCommand(TaskDto Task) : IRequest;
