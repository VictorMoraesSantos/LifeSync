namespace LifeSyncApp.Client.Models.Nutrition.MealFood
{
    public record CreateMealFoodDTO(int MealId, string Name, int QuantityInGrams, int CaloriesPerUnit);
}