using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Meal.Commands.RemoveMealFood
{
    public record RemoveMealFoodCommand(int MealId, int FoodId) : ICommand<RemoveMealFoodResult>;
    public record RemoveMealFoodResult(bool IsSuccess);
}
