namespace LifeSyncApp.Client.Models.Nutrition;

// Read DTOs
public record DiaryDTO(
    int Id,
    int UserId,
    DateOnly Date,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int TotalCalories,
    IList<MealDTO> Meals,
    IList<LiquidDTO> Liquids);

public record MealDTO(
    int Id,
    int DiaryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    string Description,
    int TotalCalories,
    IList<MealFoodDTO> MealFoods);

public record MealFoodDTO(
    int Id,
    int MealId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    int Quantity,
    int CaloriesPerUnit,
    int TotalCalories);

public record LiquidDTO(
    int Id,
    int DiaryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    int QuantityMl,
    int CaloriesPerMl,
    int TotalCalories);

// Daily Progress (objetivos e consumo)
public record DailyGoalDTO(int Calories, int QuantityMl);

public record DailyProgressDTO(
    int Id,
    int UserId,
    DateOnly Date,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int? CaloriesConsumed,
    int? LiquidsConsumed,
    DailyGoalDTO Goal);

// Commands
public record CreateDiaryCommand(int UserId, DateOnly Date);
public record UpdateDiaryCommand(int Id, DateOnly Date);

public record CreateMealCommand(int DiaryId, string Name, string Description);
public record UpdateMealCommand(int Id, string Name, string Description);

public record CreateMealFoodDTO(string Name, int QuantityInGrams, int CaloriesPerUnit);
public record CreateMealFoodCommand(int MealId, string Name, int QuantityInGrams, int CaloriesPerUnit);

public record UpdateMealFoodDTO(int Id, string Name, int QuantityInGrams, int CaloriesPerUnit);

public record CreateLiquidCommand(int DiaryId, string Name, int QuantityMl, int CaloriesPerMl);
public record UpdateLiquidCommand(int Id, string Name, int QuantityMl, int CaloriesPerMl);

// Daily progress commands
public record CreateDailyProgressCommand(int UserId, DateOnly Date, int? CaloriesConsumed, int? LiquidsConsumedMl);
public record UpdateDailyProgressCommand(int Id, int CaloriesConsumed, int LiquidsConsumedMl);
public record SetGoalRequest(DailyGoalDTO Goal);
