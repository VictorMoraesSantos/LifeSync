using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence.Configuration;

namespace TaskManager.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskLabel> TaskLabels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Error>();

            modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
            modelBuilder.ApplyConfiguration(new TaskLabelConfiguration());
        }
    }
}
