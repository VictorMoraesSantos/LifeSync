namespace LifeSyncApp.DTOs.Nutrition.DailyProgress
{
    public record UpdateDailyProgressDTO(
        int Id,
        int CaloriesConsumed,
        int LiquidsConsumedMl,
        DailyGoalDTO? Goal);
}
