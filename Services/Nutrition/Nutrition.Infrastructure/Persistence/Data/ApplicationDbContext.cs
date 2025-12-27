using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;

namespace Nutrition.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Diary> Diaries { get; set; }
        public DbSet<Liquid> Liquids { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealFood> MealFoods { get; set; }
        public DbSet<DailyProgress> DailyProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<BuildingBlocks.Results.Error>();

            modelBuilder.Entity<DailyProgress>(entity =>
            {
                entity.OwnsOne(dp => dp.Goal, goal =>
                {
                    goal.Property(g => g.Calories)
                        .HasColumnName("CaloriesGoal");

                    goal.Property(g => g.QuantityMl)
                        .HasColumnName("LiquidsGoalMl");
                });
            });
        }
    }
}
