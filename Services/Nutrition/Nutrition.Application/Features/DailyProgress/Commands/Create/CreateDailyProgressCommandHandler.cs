using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Commands.Create
{
    public class CreateDailyProgressCommandHandler : ICommandHandler<CreateDailyProgressCommand, CreateDailyProgressResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public CreateDailyProgressCommandHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<Result<CreateDailyProgressResult>> Handle(CreateDailyProgressCommand command, CancellationToken cancellationToken)
        {
            CreateDailyProgressDTO dto = new(
                command.UserId,
                command.Date,
                command.CaloriesConsumed,
                command.LiquidsConsumedMl);

            var result = await _dailyProgressService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateDailyProgressResult>(result.Error!);

            return Result.Success(new CreateDailyProgressResult(result.Value!));
        }
    }
}
