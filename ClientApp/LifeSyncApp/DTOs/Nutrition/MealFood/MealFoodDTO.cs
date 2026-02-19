namespace LifeSyncApp.DTOs.Nutrition.MealFood
{
    public record MealFoodDTO(
        int Id,
        int MealId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        int Quantity,
        int CaloriesPerUnit,
        int TotalCalories);
}
