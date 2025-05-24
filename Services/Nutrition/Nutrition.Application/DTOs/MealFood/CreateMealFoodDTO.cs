namespace Nutrition.Application.DTOs.MealFood
{
    public record CreateMealFoodDTO(
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit);
}
