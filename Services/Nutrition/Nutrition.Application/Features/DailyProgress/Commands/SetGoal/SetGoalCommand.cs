using MediatR;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Commands.SetGoal
{
    public record SetGoalCommand(int DailyProgressId, DailyGoalDTO Goal) : IRequest<SetGoalResult>;
    public record SetGoalResult(bool IsSuccess);
}
