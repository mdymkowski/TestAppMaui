using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestAppMaui.Infrastructure.Data.Migrations;

public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Tasks",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                Title = table.Column<string>(maxLength: 256, nullable: false),
                Description = table.Column<string>(maxLength: 1024, nullable: true),
                DueDate = table.Column<DateTime>(nullable: true),
                IsCompleted = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tasks", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Tasks");
    }
}
