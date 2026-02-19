using LifeSyncApp.DTOs.Nutrition.MealFood;

namespace LifeSyncApp.DTOs.Nutrition.Meal
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
