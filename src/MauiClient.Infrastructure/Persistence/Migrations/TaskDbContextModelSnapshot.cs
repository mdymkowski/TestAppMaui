using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using TestAppMaui.MauiClient.Infrastructure.Persistence;

#nullable disable

namespace TestAppMaui.MauiClient.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(TaskDbContext))]
    partial class TaskDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("TestAppMaui.MauiClient.Domain.Tasks.TaskItem", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                b.Property<string>("Description")
                    .HasColumnType("TEXT")
                    .HasMaxLength(250);

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("Tasks", (string)null);
            });
#pragma warning restore 612, 618
        }
    }
}
