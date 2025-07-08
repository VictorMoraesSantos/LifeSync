using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.MealFood.Commands.Create
{
    public class CreateMealFoodCommandHandler : ICommandHandler<CreateMealFoodCommand, CreateMealFoodResult>
    {
        private readonly IMealFoodService _mealFoodService;
        private readonly IMealService _mealService;

        public CreateMealFoodCommandHandler(IMealFoodService mealFoodService, IMealService mealService)
        {
            _mealFoodService = mealFoodService;
            _mealService = mealService;
        }

        public async Task<Result<CreateMealFoodResult>> Handle(CreateMealFoodCommand command, CancellationToken cancellationToken)
        {
            var meal = await _mealService.GetByIdAsync(command.MealId, cancellationToken);
            if (!meal.IsSuccess)
                return Result<CreateMealFoodResult>.Failure(meal.Error!);

            CreateMealFoodDTO dto = new(
                command.Name,
                command.QuantityInGrams,
                command.CaloriesPerUnit);

            var result = await _mealFoodService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result<CreateMealFoodResult>.Failure(result.Error!);

            return Result.Success(new CreateMealFoodResult(result.Value));
        }
    }
}
