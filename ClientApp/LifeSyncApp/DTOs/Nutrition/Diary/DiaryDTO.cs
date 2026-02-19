using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.Meal;

namespace LifeSyncApp.DTOs.Nutrition.Diary
{
    public record DiaryDTO(
        int Id,
        int UserId,
        DateOnly Date,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int TotalCalories,
        List<MealDTO> Meals,
        List<LiquidDTO> Liquids);
}
