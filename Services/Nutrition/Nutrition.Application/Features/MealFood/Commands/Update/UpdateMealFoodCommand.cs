using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.MealFood.Commands.Update
{
    public record UpdateMealFoodCommand(
        int Id,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit)
        : ICommand<UpdateMealFoodResult>;
    public record UpdateMealFoodResult(bool IsSuccess);
}
