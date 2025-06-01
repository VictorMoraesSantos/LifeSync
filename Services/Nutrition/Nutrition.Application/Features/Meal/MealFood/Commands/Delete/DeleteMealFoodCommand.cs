using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.Meal.MealFood.Commands.Delete
{
    public record DeleteMealFoodCommand(int Id) : IRequest<DeleteMealFoodResult>;
    public record DeleteMealFoodResult(bool IsSuccess);
}
