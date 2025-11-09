namespace LifeSyncApp.Client.Models.Nutrition.DailyProgress
{
    public record DailyProgressDTO(
        int Id,
        int UserId,
        DateOnly Date,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int? CaloriesConsumed,
        int? LiquidsConsumed,
        DailyGoalDTO Goal);

    public record DailyGoalDTO(int Calories, int QuantityMl);
}