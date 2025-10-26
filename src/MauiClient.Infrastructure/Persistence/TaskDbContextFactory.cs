using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAppMaui.MauiClient.Infrastructure.Persistence;

namespace MauiClient.Infrastructure.Persistence
{
    public sealed class TaskDbContextFactory : IDesignTimeDbContextFactory<TaskDbContext>
    {
        public TaskDbContext CreateDbContext(string[] args)
        {
            // Prosta, lokalna baza tylko na czas generowania/uruchamiania migracji z CLI
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "design-time.db");
            var options = new DbContextOptionsBuilder<TaskDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new TaskDbContext(options);
        }
    }
}
