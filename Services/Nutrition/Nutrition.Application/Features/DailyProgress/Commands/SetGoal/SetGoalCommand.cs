using BuildingBlocks.CQRS.Requests.Commands;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Commands.SetGoal
{
    public record SetGoalCommand(int DailyProgressId, DailyGoalDTO Goal) : ICommand<SetGoalResult>;
    public record SetGoalResult(bool IsSuccess);
}
