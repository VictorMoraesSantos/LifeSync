using MediatR;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFood.Commands.Create
{
    public class CreateMealFoodCommandHandler : IRequestHandler<CreateMealFoodCommand, CreateMealFoodResponse>
    {
        private readonly IMealFoodService _mealFoodService;
        private readonly IMealService _mealService;

        public CreateMealFoodCommandHandler(IMealFoodService mealFoodService, IMealService mealService)
        {
            _mealFoodService = mealFoodService;
            _mealService = mealService;
        }

        public async Task<CreateMealFoodResponse> Handle(CreateMealFoodCommand command, CancellationToken cancellationToken)
        {
            MealDTO? meal = await _mealService.GetByIdAsync(command.MealId, cancellationToken);
            if (meal == null)
                return new CreateMealFoodResponse(false);

            CreateMealFoodDTO dto = new(
                command.MealId,
                command.Name,
                command.QuantityInGrams,
                command.CaloriesPerUnit);

            bool result = await _mealFoodService.CreateAsync(dto, cancellationToken);
            CreateMealFoodResponse response = new(result);
            return response;
        }
    }
}
