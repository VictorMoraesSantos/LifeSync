using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Commands.Delete
{
    public class DeleteDailyProgressCommandHandler : ICommandHandler<DeleteDailyProgressCommand, DeleteDailyProgressResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public DeleteDailyProgressCommandHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<Result<DeleteDailyProgressResult>> Handle(DeleteDailyProgressCommand command, CancellationToken cancellationToken)
        {
            var result = await _dailyProgressService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteDailyProgressResult>(result.Error!);

            return Result.Success(new DeleteDailyProgressResult(result.Value!));
        }
    }
}
