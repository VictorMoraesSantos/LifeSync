using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.MealFood.Commands.Create
{
    public record CreateMealFoodCommand(
        int MealId,
        int FoodId,
        int Quantity)
        : ICommand<CreateMealFoodResult>;
    public record CreateMealFoodResult(int Id);
}
