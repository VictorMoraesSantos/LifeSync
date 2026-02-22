using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.MealFood.Commands.Update
{
    public class UpdateMealFoodCommandHandler : ICommandHandler<UpdateMealFoodCommand, UpdateMealFoodResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public UpdateMealFoodCommandHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<Result<UpdateMealFoodResult>> Handle(UpdateMealFoodCommand command, CancellationToken cancellationToken)
        {
            UpdateMealFoodDTO mealFood = new(
                command.Id,
                command.Code,
                command.Name,
                command.Calories,
                command.Protein,
                command.Lipids,
                command.Carbohydrates,
                command.Calcium,
                command.Magnesium,
                command.Iron,
                command.Sodium,
                command.Potassium,
                command.Quantity);

            var result = await _mealFoodService.UpdateAsync(mealFood);
            if (!result.IsSuccess)
                return Result<UpdateMealFoodResult>.Failure(result.Error!);

            return Result.Success(new UpdateMealFoodResult(result.Value));
        }
    }
}
