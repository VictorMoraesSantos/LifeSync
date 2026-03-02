namespace LifeSyncApp.DTOs.Nutrition.MealFood
{
    public record MealFoodDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int MealId,
        int FoodId,
        string Name,
        int Calories,
        decimal? Protein,
        decimal? Lipids,
        decimal? Carbohydrates,
        decimal? Calcium,
        decimal? Magnesium,
        decimal? Iron,
        decimal? Sodium,
        decimal? Potassium,
        int Quantity,
        decimal TotalCalories);
}
