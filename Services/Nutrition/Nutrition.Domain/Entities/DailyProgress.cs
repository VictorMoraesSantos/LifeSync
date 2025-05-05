using Nutrition.Domain.ValueObjects;
using System;

namespace Nutrition.Domain.Entities
{
    public class DailyProgress
    {
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; }

        public int CaloriesConsumed { get; private set; }
        public int LiquidsConsumedMl { get; private set; }

        public DailyGoal Goal { get; private set; }

        public DailyProgress(int userId, DateOnly date, int caloriesGoal, int liquidsGoalMl)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be positive.", nameof(userId));
            if (caloriesGoal < 0)
                throw new ArgumentOutOfRangeException(nameof(caloriesGoal), "Calories goal cannot be negative.");
            if (liquidsGoalMl < 0)
                throw new ArgumentOutOfRangeException(nameof(liquidsGoalMl), "Liquids goal cannot be negative.");

            UserId = userId;
            Date = date;
            Goal = new DailyGoal(caloriesGoal, liquidsGoalMl);
            CaloriesConsumed = 0;
            LiquidsConsumedMl = 0;
        }

        // Atualiza as calorias consumidas (incremental)
        public void AddCalories(int calories)
        {
            if (calories < 0)
                throw new ArgumentOutOfRangeException(nameof(calories), "Calories to add cannot be negative.");
            CaloriesConsumed += calories;
        }

        // Atualiza os líquidos consumidos (incremental)
        public void AddLiquids(int quantityMl)
        {
            if (quantityMl < 0)
                throw new ArgumentOutOfRangeException(nameof(quantityMl), "Liquids to add cannot be negative.");
            LiquidsConsumedMl += quantityMl;
        }

        // Percentual de calorias consumidas em relação à meta (0 a 100)
        public int GetCaloriesProgressPercentage()
        {
            if (Goal.Calories == 0) return 0;
            return Math.Min((int)((CaloriesConsumed / (float)Goal.Calories) * 100), 100);
        }

        // Percentual de líquidos consumidos em relação à meta (0 a 100)
        public int GetLiquidsProgressPercentage()
        {
            if (Goal.QuantityMl == 0) return 0;
            return Math.Min((int)((LiquidsConsumedMl / (float)Goal.QuantityMl) * 100), 100);
        }

        // Verifica se as metas foram atingidas
        public bool IsCaloriesGoalMet()
        {
            return CaloriesConsumed >= Goal.Calories;
        }

        public bool IsLiquidsGoalMet()
        {
            return LiquidsConsumedMl >= Goal.QuantityMl;
        }

        // Permite redefinir metas (se necessário)
        public void UpdateGoals(int newCaloriesGoal, int newLiquidsGoalMl)
        {
            if (newCaloriesGoal < 0)
                throw new ArgumentOutOfRangeException(nameof(newCaloriesGoal), "Calories goal cannot be negative.");
            if (newLiquidsGoalMl < 0)
                throw new ArgumentOutOfRangeException(nameof(newLiquidsGoalMl), "Liquids goal cannot be negative.");

            Goal = new DailyGoal(newCaloriesGoal, newLiquidsGoalMl);
        }
    }
}