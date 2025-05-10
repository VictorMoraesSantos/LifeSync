namespace Nutrition.Application.DTOs.MealFoods
{
    public record CreateMealFoodDTO(
        int MealId,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit);
}
