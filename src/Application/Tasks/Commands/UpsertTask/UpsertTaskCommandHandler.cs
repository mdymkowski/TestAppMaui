using MediatR;
using TestAppMaui.Application.Common.Interfaces;
using TestAppMaui.Domain.Aggregates.Tasks;

namespace TestAppMaui.Application.Tasks.Commands.UpsertTask;

public sealed class UpsertTaskCommandHandler : IRequestHandler<UpsertTaskCommand>
{
    private readonly IRepository<TaskItem> _repository;

    public UpsertTaskCommandHandler(IRepository<TaskItem> repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpsertTaskCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Task);

        var dto = request.Task;
        var existing = await _repository.GetByIdAsync(dto.Id, cancellationToken).ConfigureAwait(false);

        if (existing is null)
        {
            var task = TaskItem.FromExisting(dto.Id, dto.Title, dto.Description, dto.DueDate, dto.IsCompleted);
            await _repository.AddAsync(task, cancellationToken).ConfigureAwait(false);
            return;
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
}
