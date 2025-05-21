namespace Nutrition.Application.DTOs.DailyProgress
{
    public record UpdateDailyProgressDTO(
        int Id,
        int? CaloriesConsumed,
        int? LiquidsConsumedMl,
        DailyGoalDTO? Goal);
}
