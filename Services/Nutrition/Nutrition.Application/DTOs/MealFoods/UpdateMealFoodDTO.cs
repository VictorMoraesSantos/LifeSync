namespace Nutrition.Application.DTOs.MealFoods
{
    public record UpdateMealFoodDTO(
        int Id,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit);
}
