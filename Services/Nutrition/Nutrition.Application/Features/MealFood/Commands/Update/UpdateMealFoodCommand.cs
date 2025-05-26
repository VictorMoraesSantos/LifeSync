using MediatR;

namespace Nutrition.Application.Features.MealFood.Commands.Update
{
    public record UpdateMealFoodCommand(
        int Id,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit)
        : IRequest<UpdateMealFoodResult>;
    public record UpdateMealFoodResult(bool IsSuccess);
}
