namespace Nutrition.Application.DTOs.MealFood
{
    public record CreateMealFoodDTO(
        int MealId,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit);
}
