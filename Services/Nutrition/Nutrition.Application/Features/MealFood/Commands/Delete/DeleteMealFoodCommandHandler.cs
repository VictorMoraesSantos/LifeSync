using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.MealFood.Commands.Delete
{
    public class DeleteMealFoodCommandHandler : ICommandHandler<DeleteMealFoodCommand, DeleteMealFoodResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public DeleteMealFoodCommandHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<Result<DeleteMealFoodResult>> Handle(DeleteMealFoodCommand command, CancellationToken cancellationToken)
        {
            var result = await _mealFoodService.DeleteAsync(command.Id);
            if(!result.IsSuccess)
                return Result<DeleteMealFoodResult>.Failure(result.Error!);

            return Result.Success(new DeleteMealFoodResult(result.Value));
        }
    }
}
