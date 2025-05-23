using MediatR;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.DailyProgress.Commands.SetGoal
{
    public class SetGoalCommandHandler : IRequestHandler<SetGoalCommand, SetGoalResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public SetGoalCommandHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<SetGoalResult> Handle(SetGoalCommand command, CancellationToken cancellationToken)
        {
            DailyGoalDTO dto = command.Goal;
            await _dailyProgressService.SetGoalAsync(command.DailyProgressId, dto, cancellationToken);
            SetGoalResult response = new(true);
            return response;
        }
    }
}
