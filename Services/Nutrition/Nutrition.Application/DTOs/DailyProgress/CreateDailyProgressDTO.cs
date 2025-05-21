namespace Nutrition.Application.DTOs.DailyProgress
{
    public record CreateDailyProgressDTO(
        int UserId,
        DateOnly Date,
        int? CaloriesConsumed,
        int? LiquidsConsumedMl,
        DailyGoalDTO? Goal);
}
