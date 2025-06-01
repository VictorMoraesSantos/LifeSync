using BuildingBlocks.CQRS.Request;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.RemoveMealFood
{
    public class RemoveMealFoodCommandHandler : IRequestHandler<RemoveMealFoodCommand, RemoveMealFoodResult>
    {
        private readonly IMealService _mealService;

        public RemoveMealFoodCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<RemoveMealFoodResult> Handle(RemoveMealFoodCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mealService.RemoveMealFoodAsync(command.MealId, command.FoodId, cancellationToken);

            return new RemoveMealFoodResult(result);
        }
    }
}
