namespace LifeSyncApp.Models.Nutrition.DailyProgress
{
    public record UpdateDailyProgressDTO(
        int Id,
        int CaloriesConsumed,
        int LiquidsConsumedMl,
        DailyGoalDTO? Goal);
}
