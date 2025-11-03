namespace LifeSyncApp.Client.Models.Nutrition
{
    public class DiaryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateOnly Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalCalories { get; set; }
        public List<MealDto> Meals { get; set; } = new();
        public List<LiquidDto> Liquids { get; set; } = new();
    }

    public class CreateDiaryRequest
    {
        public int UserId { get; set; }
        public DateOnly Date { get; set; }
    }

    public class MealDto
    {
        public int Id { get; set; }
        public int DiaryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalCalories { get; set; }
        public List<MealFoodDto> MealFoods { get; set; } = new();
    }

    public class CreateMealRequest
    {
        public int DiaryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateMealRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class MealFoodDto
    {
        public int Id { get; set; }
        public int MealId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int Calories { get; set; }
    }

    public class CreateMealFoodRequest
    {
        public string FoodName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int Calories { get; set; }
    }

    public class LiquidDto
    {
        public int Id { get; set; }
        public int DiaryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public int QuantityMl { get; set; }
        public int CaloriesPerMl { get; set; }
        public int TotalCalories { get; set; }
    }

    public class CreateLiquidRequest
    {
        public int DiaryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int QuantityMl { get; set; }
        public int CaloriesPerMl { get; set; }
    }

    public class UpdateLiquidRequest
    {
        public string Name { get; set; } = string.Empty;
        public int QuantityMl { get; set; }
        public int CaloriesPerMl { get; set; }
    }

    public class DailyProgressDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateOnly Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CaloriesConsumed { get; set; }
        public int? LiquidsConsumed { get; set; }
        public DailyGoalDto Goal { get; set; } = new();
    }

    public class CreateDailyProgressRequest
    {
        public int UserId { get; set; }
        public DateOnly Date { get; set; }
        public int? CaloriesConsumed { get; set; }
        public int? LiquidsConsumed { get; set; }
        public DailyGoalDto Goal { get; set; } = new();
    }

    public class UpdateDailyProgressRequest
    {
        public int? CaloriesConsumed { get; set; }
        public int? LiquidsConsumed { get; set; }
        public DailyGoalDto? Goal { get; set; }
    }

    public class DailyGoalDto
    {
        public int Calories { get; set; }
        public int QuantityMl { get; set; }
    }
}
