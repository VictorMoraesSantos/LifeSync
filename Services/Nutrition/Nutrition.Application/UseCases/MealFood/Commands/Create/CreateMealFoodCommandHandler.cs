using MediatR;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFood.Commands.Create
{
    public class CreateMealFoodCommandHandler : IRequestHandler<CreateMealFoodCommand, CreateMealFoodResult>
    {
        private readonly IMealFoodService _mealFoodService;
        private readonly IMealService _mealService;

        public CreateMealFoodCommandHandler(IMealFoodService mealFoodService, IMealService mealService)
        {
            _mealFoodService = mealFoodService;
            _mealService = mealService;
        }

        public async Task<CreateMealFoodResult> Handle(CreateMealFoodCommand command, CancellationToken cancellationToken)
        {
            MealDTO? meal = await _mealService.GetByIdAsync(command.MealId, cancellationToken);
            if (meal == null)
                return new CreateMealFoodResult(false);

            CreateMealFoodDTO dto = new(
                command.Name,
                command.QuantityInGrams,
                command.CaloriesPerUnit);

            bool result = await _mealFoodService.CreateAsync(dto, cancellationToken);
            CreateMealFoodResult response = new(result);
            return response;
        }
    }
}
