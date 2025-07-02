
using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.MealFood.Commands.Create
{
    public record CreateMealFoodCommand(
        int MealId,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit)
        : ICommand<CreateMealFoodResult>;
    public record CreateMealFoodResult(int Id);
}
