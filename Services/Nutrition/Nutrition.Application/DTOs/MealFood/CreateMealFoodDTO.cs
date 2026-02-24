namespace Nutrition.Application.DTOs.MealFood
{
    public record CreateMealFoodDTO(
        int MealId,
        int FoodId,
        int Quantity);
}
