using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.Delete
{
    public class DeleteMealCommandHandler : ICommandHandler<DeleteMealCommand, DeleteMealResult>
    {
        private readonly IMealService _mealService;

        public DeleteMealCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<Result<DeleteMealResult>> Handle(DeleteMealCommand command, CancellationToken cancellationToken)
        {
            var result = await _mealService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteMealResult>(result.Error!);

            return Result.Success(new DeleteMealResult(result.Value));
        }
    }
}
