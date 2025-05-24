using MediatR;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Meal.Commands.AddMealFood
{
    public class AddMealFoodCommandHandler : IRequestHandler<AddMealFoodCommand, AddMealFoodResult>
    {
        private readonly IMealService _mealService;

        public AddMealFoodCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<AddMealFoodResult> Handle(AddMealFoodCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mealService.AddMealFoodAsync(command.MealId, command.MealFood, cancellationToken);
            return new AddMealFoodResult(result);
        }
    }
}
