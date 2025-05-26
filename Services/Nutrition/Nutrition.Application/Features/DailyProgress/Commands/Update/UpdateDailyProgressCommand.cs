using MediatR;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Commands.Update
{
    public record UpdateDailyProgressCommand(
        int Id,
        int CaloriesConsumed,
        int LiquidsConsumedMl,
        DailyGoalDTO? Goal)
    : IRequest<UpdateDailyProgressResult>;
    public record class UpdateDailyProgressResult(bool IsSuccess);
}
