using System.Collections.Generic;
using MediatR;
using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.Gateway.Application.Tasks.Commands.ReplaceTasks;

public sealed record ReplaceTasksCommand(IReadOnlyCollection<TaskDto> Tasks) : IRequest;
