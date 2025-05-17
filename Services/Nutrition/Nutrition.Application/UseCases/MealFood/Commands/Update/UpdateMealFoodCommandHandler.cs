using MediatR;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFood.Commands.Update
{
    public class UpdateMealFoodCommandHandler : IRequestHandler<UpdateMealFoodCommand, UpdateMealFoodResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public UpdateMealFoodCommandHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<UpdateMealFoodResult> Handle(UpdateMealFoodCommand command, CancellationToken cancellationToken)
        {
            UpdateMealFoodDTO mealFood = new (
                command.Id,
                command.Name,
                command.QuantityInGrams,
                command.CaloriesPerUnit);

            bool result =await _mealFoodService.UpdateAsync(mealFood);
            UpdateMealFoodResult response = new(result);
            return response;
        }
    }
}
