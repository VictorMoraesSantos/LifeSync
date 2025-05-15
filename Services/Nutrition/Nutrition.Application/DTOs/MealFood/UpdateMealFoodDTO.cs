namespace Nutrition.Application.DTOs.MealFood
{
    public record UpdateMealFoodDTO(
        int Id,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit);
}
