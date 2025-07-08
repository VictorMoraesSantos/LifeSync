
using BuildingBlocks.CQRS.Commands;

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
