using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Commands.SetGoal
{
    public record SetGoalCommand(int DailyProgressId, DailyGoalDTO Goal) : ICommand<SetGoalResult>;
    public record SetGoalResult(bool IsSuccess);
}
