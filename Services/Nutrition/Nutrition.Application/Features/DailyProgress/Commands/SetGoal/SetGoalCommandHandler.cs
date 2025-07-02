using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Commands.SetGoal
{
    public class SetGoalCommandHandler : ICommandHandler<SetGoalCommand, SetGoalResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public SetGoalCommandHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<Result<SetGoalResult>> Handle(SetGoalCommand command, CancellationToken cancellationToken)
        {
            DailyGoalDTO dto = command.Goal;

            var result = await _dailyProgressService.SetGoalAsync(command.DailyProgressId, dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<SetGoalResult>(result.Error!);

            return Result.Success(new SetGoalResult(true));
        }
    }
}
