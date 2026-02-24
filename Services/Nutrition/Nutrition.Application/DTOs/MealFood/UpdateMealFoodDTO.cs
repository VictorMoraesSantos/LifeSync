namespace Nutrition.Application.DTOs.MealFood
{
    public record UpdateMealFoodDTO(
        int Id,
        int FoodId,
        int Quantity);
}
