using MediatR;

namespace Nutrition.Application.UseCases.MealFood.Commands.Create
{
    public record CreateMealFoodCommand(
        int MealId,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit)
        : IRequest<CreateMealFoodResponse>;
    public record CreateMealFoodResponse(bool IsSuccess);
}
