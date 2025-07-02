using BuildingBlocks.CQRS.Commands;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Commands.Update
{
    public record UpdateDailyProgressCommand(
        int Id,
        int CaloriesConsumed,
        int LiquidsConsumedMl,
        DailyGoalDTO? Goal)
    : ICommand<UpdateDailyProgressResult>;
    public record class UpdateDailyProgressResult(bool IsSuccess);
}
