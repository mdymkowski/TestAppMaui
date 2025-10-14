using System.Collections.Generic;
using MediatR;
using TestAppMaui.Application.Common.Models;

namespace TestAppMaui.Application.Tasks.Commands.ReplaceTasks;

public sealed record ReplaceTasksCommand(IReadOnlyCollection<TaskDto> Tasks) : IRequest;
