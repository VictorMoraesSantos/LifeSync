using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.Meal.Commands.RemoveMealFood
{
    public record RemoveMealFoodCommand(int MealId, int FoodId) : IRequest<RemoveMealFoodResult>;
    public record RemoveMealFoodResult(bool IsSuccess);
}
