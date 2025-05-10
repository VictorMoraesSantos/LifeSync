using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;

namespace Nutrition.Infrastructure.Data
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

            modelBuilder.Entity<DailyProgress>(entity =>
            {
                entity.OwnsOne(dp => dp.Goal, goal =>
                {
                    goal.Property(g => g.Calories)
                        .HasColumnName("CaloriesGoal")
                        .IsRequired();

                    goal.Property(g => g.QuantityMl)
                        .HasColumnName("LiquidsGoalMl")
                        .IsRequired();
                });
            });
        }
    }
}
