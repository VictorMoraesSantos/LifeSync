using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.RemoveMealFood
{
    public class RemoveMealFoodCommandHandler : ICommandHandler<RemoveMealFoodCommand, RemoveMealFoodResult>
    {
        private readonly IMealService _mealService;

        public RemoveMealFoodCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<Result<RemoveMealFoodResult>> Handle(RemoveMealFoodCommand command, CancellationToken cancellationToken)
        {
            var result = await _mealService.RemoveMealFoodAsync(command.MealId, command.FoodId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<RemoveMealFoodResult>(result.Error!);

            return Result.Success(new RemoveMealFoodResult(result.Value));
        }
    }
}
