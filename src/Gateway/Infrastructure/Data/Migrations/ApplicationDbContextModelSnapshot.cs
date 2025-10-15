using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TestAppMaui.Gateway.Infrastructure.Data;

#nullable disable

namespace TestAppMaui.Gateway.Infrastructure.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.4");

        modelBuilder.Entity("TestAppMaui.Gateway.Domain.Aggregates.Tasks.TaskItem", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd();

            b.Property<string?>("Description")
                .HasMaxLength(1024);

            b.Property<DateTime?>("DueDate");

            b.Property<bool>("IsCompleted");

            b.Property<string>("Title")
                .IsRequired()
                .HasMaxLength(256);

            b.HasKey("Id");

            b.ToTable("Tasks");
        });
#pragma warning restore 612, 618
    }
}
