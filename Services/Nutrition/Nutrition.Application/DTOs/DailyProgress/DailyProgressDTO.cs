using Core.Application.DTO;

namespace Nutrition.Application.DTOs.DailyProgress
{
    public record DailyProgressDTO(
        int Id,
        int UserId,
        DateOnly Date,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int? CaloriesConsumed,
        int? LiquidsConsumed,
        DailyGoalDTO Goal)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
