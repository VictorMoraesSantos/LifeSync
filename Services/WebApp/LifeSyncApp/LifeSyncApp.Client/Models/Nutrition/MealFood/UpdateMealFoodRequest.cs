namespace LifeSyncApp.Client.Models.Nutrition.MealFood
{
    // Request body for updating a MealFood (Id is passed in the URL)
    public record UpdateMealFoodRequest(string Name, int QuantityInGrams, int CaloriesPerUnit);
}
