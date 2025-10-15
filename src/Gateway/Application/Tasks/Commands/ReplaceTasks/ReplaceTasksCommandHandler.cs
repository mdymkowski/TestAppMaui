using System.Collections.Generic;
using System.Linq;
using MediatR;
using TestAppMaui.Gateway.Application.Common.Interfaces;
using TestAppMaui.Gateway.Domain.Aggregates.Tasks;

namespace TestAppMaui.Gateway.Application.Tasks.Commands.ReplaceTasks;

public sealed class ReplaceTasksCommandHandler : IRequestHandler<ReplaceTasksCommand>
{
    private readonly IRepository<TaskItem> _repository;

    public ReplaceTasksCommandHandler(IRepository<TaskItem> repository)
    {
        _repository = repository;
    }

    public async Task Handle(ReplaceTasksCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Tasks);

        var desiredTasks = request.Tasks;
        var desiredLookup = desiredTasks.ToDictionary(task => task.Id);

        var existingTasks = await _repository.ListAsync(cancellationToken).ConfigureAwait(false);
        var existingIds = existingTasks.Select(task => task.Id).ToHashSet();

        foreach (var existing in existingTasks)
        {
            if (!desiredLookup.TryGetValue(existing.Id, out var dto))
            {
                await _repository.DeleteAsync(existing, cancellationToken).ConfigureAwait(false);
                continue;
            }

            existing.UpdateDetails(dto.Title, dto.Description, dto.DueDate);

            if (dto.IsCompleted)
            {
                existing.MarkComplete();
            }
            else
            {
                existing.MarkIncomplete();
            }

            await _repository.UpdateAsync(existing, cancellationToken).ConfigureAwait(false);
        }

        foreach (var dto in desiredTasks)
        {
            if (existingIds.Contains(dto.Id))
            {
                continue;
            }

            var task = TaskItem.FromExisting(dto.Id, dto.Title, dto.Description, dto.DueDate, dto.IsCompleted);
            await _repository.AddAsync(task, cancellationToken).ConfigureAwait(false);
        }
    }
}
