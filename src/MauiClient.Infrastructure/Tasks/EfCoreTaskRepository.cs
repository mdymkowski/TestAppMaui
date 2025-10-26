using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TestAppMaui.MauiClient.Application.Abstractions;
using TestAppMaui.MauiClient.Domain.Tasks;
using TestAppMaui.MauiClient.Infrastructure.Persistence;

namespace TestAppMaui.MauiClient.Infrastructure.Tasks;

public sealed class EfCoreTaskRepository : ITaskRepository
{
    private readonly IDbContextFactory<TaskDbContext> _contextFactory;

    public EfCoreTaskRepository(IDbContextFactory<TaskDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

        var tasks = await context.Tasks
            .AsNoTracking()
            .OrderBy(task => task.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return tasks;
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

        context.Tasks.Add(task);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
