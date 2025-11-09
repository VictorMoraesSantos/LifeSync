namespace LifeSyncApp.Client.Models.Nutrition.Meal
{
    public record MealDTO(
        int Id,
        int DiaryId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string Description,
        int TotalCalories,
        IList<MealFoodDTO> MealFoods);
}