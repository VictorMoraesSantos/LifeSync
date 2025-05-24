using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Domain.Entities
{
    public class DailyProgress : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public int CaloriesConsumed { get; private set; } = 0;
        public int LiquidsConsumedMl { get; private set; } = 0;

        public DailyGoal? Goal { get; private set; } = new(0, 0);

        protected DailyProgress() { }

        public DailyProgress(int userId, DateOnly date, int caloriesConsumed, int liquidsConsumedMl)
        {
            if (userId <= 0)
                throw new DomainException($"{nameof(userId)} must be valid.");
            Validate(caloriesConsumed);
            Validate(liquidsConsumedMl);
            UserId = userId;
            Date = date;
            CaloriesConsumed = caloriesConsumed;
            LiquidsConsumedMl = liquidsConsumedMl;
        }

        public void SetGoal(DailyGoal goal)
        {
            if (goal == null)
                throw new DomainException($"{nameof(goal)} cannot be null.");
            Goal = goal;
        }

        public void ResetGoal()
        {
            Goal = new DailyGoal(0, 0);
        }

        public void SetConsumed(int caloriesConsumed, int liquidsConsumedMl)
        {
            Validate(caloriesConsumed);
            Validate(liquidsConsumedMl);
            CaloriesConsumed = caloriesConsumed;
            LiquidsConsumedMl = liquidsConsumedMl;
        }

        public void AddCalories(int calories)
        {
            Validate(calories);
            CaloriesConsumed += calories;
        }

        public void AddLiquidsQuantity(int liquidsMl)
        {
            Validate(liquidsMl);
            LiquidsConsumedMl += liquidsMl;
        }

        public int GetCaloriesProgressPercentage()
        {
            if (Goal.Calories == 0) return 0;
            return Math.Min((int)((CaloriesConsumed / (float)Goal.Calories) * 100), 100);
        }

        public int GetLiquidsProgressPercentage()
        {
            if (Goal.QuantityMl == 0) return 0;
            return Math.Min((int)((LiquidsConsumedMl / (float)Goal.QuantityMl) * 100), 100);
        }

        public bool IsGoalMet()
        {
            return (IsCaloriesGoalMet() && IsLiquidsGoalMet());
        }

        public bool IsCaloriesGoalMet()
        {
            return CaloriesConsumed >= Goal.Calories;
        }

        public bool IsLiquidsGoalMet()
        {
            return LiquidsConsumedMl >= Goal.QuantityMl;
        }

        private void Validate(int? value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException($"{nameof(value)} cannot be negative.");
        }
    }
}