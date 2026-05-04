using LifeSyncApp.Models.Nutrition.MealFood;

namespace LifeSyncApp.Models.Nutrition.Meal
{
    public record MealDTO(
        int Id,
        int DiaryId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string Description,
        int TotalCalories,
        List<MealFoodDTO> MealFoods);
}
