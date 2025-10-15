using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestAppMaui.Gateway.Domain.Aggregates.Tasks;

namespace TestAppMaui.Gateway.Infrastructure.Configuration;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(task => task.Description)
            .HasMaxLength(1024);
    }
}
