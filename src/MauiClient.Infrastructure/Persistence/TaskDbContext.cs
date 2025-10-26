using Microsoft.EntityFrameworkCore;
using TestAppMaui.MauiClient.Domain.Tasks;

namespace TestAppMaui.MauiClient.Infrastructure.Persistence;

public sealed class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Name)
                .IsRequired()
                .HasColumnType("TEXT");
            entity.Property(task => task.Description)
                .HasMaxLength(TaskItem.DescriptionMaxLength)
                .HasColumnType("TEXT");
        });
    }
}
