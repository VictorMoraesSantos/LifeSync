using Gym.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<CompletedExercise> CompletedExercises { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<RoutineExercise> RoutineExercises { get; set; }
        public DbSet<Routine> Routines { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
