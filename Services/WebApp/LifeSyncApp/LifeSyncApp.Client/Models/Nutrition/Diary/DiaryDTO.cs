namespace LifeSyncApp.Client.Models.Nutrition.Diary
{
    public record DiaryDTO(
        int Id,
        int UserId,
        DateOnly Date,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int TotalCalories,
        IList<MealDTO> Meals,
        IList<LiquidDTO> Liquids);
}