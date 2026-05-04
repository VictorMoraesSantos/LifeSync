using LifeSyncApp.Models.Nutrition.Liquid;
using LifeSyncApp.Models.Nutrition.Meal;

namespace LifeSyncApp.Models.Nutrition.Diary
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
