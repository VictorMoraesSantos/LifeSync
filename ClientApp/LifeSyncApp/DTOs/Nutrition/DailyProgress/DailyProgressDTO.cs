using System.Text.Json.Serialization;

namespace LifeSyncApp.DTOs.Nutrition.DailyProgress
{
    public record DailyProgressDTO(
        int Id,
        int UserId,
        DateOnly Date,
        int? CaloriesConsumed,
        [property: JsonPropertyName("liquidsConsumed")] int? LiquidsConsumedMl,
        DailyGoalDTO? Goal);
}
