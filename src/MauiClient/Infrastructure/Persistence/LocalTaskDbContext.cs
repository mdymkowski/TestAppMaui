using Microsoft.EntityFrameworkCore;
using TestAppMaui.MauiClient.Domain.Tasks;

namespace TestAppMaui.MauiClient.Infrastructure.Persistence;

public sealed class LocalTaskDbContext : DbContext
{
    public LocalTaskDbContext(DbContextOptions<LocalTaskDbContext> options)
        : base(options)
    {
    }

    public DbSet<LocalTask> Tasks => Set<LocalTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocalTask>(entity =>
        {
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Title).IsRequired().HasMaxLength(256);
            entity.Property(task => task.Description).HasMaxLength(1024);
            entity.Property(task => task.DueDate);
            entity.Property(task => task.IsCompleted);
        });
    }
}
