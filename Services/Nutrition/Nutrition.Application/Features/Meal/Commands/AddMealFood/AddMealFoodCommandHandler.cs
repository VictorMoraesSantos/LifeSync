using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.AddMealFood
{
    public class AddMealFoodCommandHandler : ICommandHandler<AddMealFoodCommand, AddMealFoodResult>
    {
        private readonly IMealService _mealService;

        public AddMealFoodCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<Result<AddMealFoodResult>> Handle(AddMealFoodCommand command, CancellationToken cancellationToken)
        {
            var result = await _mealService.AddMealFoodAsync(command.MealId, command.MealFood, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<AddMealFoodResult>(result.Error!);

            return Result.Success(new AddMealFoodResult(result.Value));
        }
    }
}
