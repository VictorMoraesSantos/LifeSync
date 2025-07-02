using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Commands.Update
{
    public class UpdateDailyProgressCommandHandler : ICommandHandler<UpdateDailyProgressCommand, UpdateDailyProgressResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public UpdateDailyProgressCommandHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<Result<UpdateDailyProgressResult>> Handle(UpdateDailyProgressCommand command, CancellationToken cancellationToken)
        {
            UpdateDailyProgressDTO dto = new(
                command.Id,
                command.CaloriesConsumed,
                command.LiquidsConsumedMl);

            var result = await _dailyProgressService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateDailyProgressResult>(result.Error!);

            return Result.Success(new UpdateDailyProgressResult(result.Value!));
        }
    }
}
