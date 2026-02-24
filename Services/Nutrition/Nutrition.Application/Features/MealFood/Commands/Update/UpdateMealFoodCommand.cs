using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.MealFood.Commands.Update
{
    public record UpdateMealFoodCommand(
        int Id,
        int FoodId,
        int Quantity)
        : ICommand<UpdateMealFoodResult>;
    public record UpdateMealFoodResult(bool IsSuccess);
}
