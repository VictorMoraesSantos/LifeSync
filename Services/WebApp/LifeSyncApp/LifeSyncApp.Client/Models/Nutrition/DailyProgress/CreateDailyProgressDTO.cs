namespace LifeSyncApp.Client.Models.Nutrition.DailyProgress
{
    public record CreateDailyProgressDTO(int UserId, DateOnly Date, int? CaloriesConsumed, int? LiquidsConsumedMl);
}