using MediatR;
using Nutrition.Application.DTOs.MealFoods;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFoods.Commands.Update
{
    public class UpdateMealFoodCommandHandler : IRequestHandler<UpdateMealFoodCommand, UpdateMealFoodResponse>
    {
        private readonly IMealFoodService _mealFoodService;

        public UpdateMealFoodCommandHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<UpdateMealFoodResponse> Handle(UpdateMealFoodCommand command, CancellationToken cancellationToken)
        {
            UpdateMealFoodDTO mealFood = new (
                command.Id,
                command.Name,
                command.QuantityInGrams,
                command.CaloriesPerUnit);

            bool result =await _mealFoodService.UpdateAsync(mealFood);
            UpdateMealFoodResponse response = new(result);
            return response;
        }
    }
}
