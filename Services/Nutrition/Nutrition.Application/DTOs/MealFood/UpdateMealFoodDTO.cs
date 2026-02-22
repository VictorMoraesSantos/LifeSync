namespace Nutrition.Application.DTOs.MealFood
{
    public record UpdateMealFoodDTO(
        int Id,
        int Code,
        string Name,
        int Calories,
        decimal Protein,
        decimal Lipids,
        decimal Carbohydrates,
        decimal Calcium,
        decimal Magnesium,
        decimal Iron,
        decimal Sodium,
        decimal Potassium,
        int Quantity);
}
