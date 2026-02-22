using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.MealFood.Commands.Delete
{
    public record DeleteMealFoodCommand(int Id) : ICommand<DeleteMealFoodResult>;
    public record DeleteMealFoodResult(bool IsSuccess);
}
