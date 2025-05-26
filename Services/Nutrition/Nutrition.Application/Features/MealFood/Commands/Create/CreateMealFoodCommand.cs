using MediatR;

namespace Nutrition.Application.Features.MealFood.Commands.Create
{
    public record CreateMealFoodCommand(
        int MealId,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit)
        : IRequest<CreateMealFoodResult>;
    public record CreateMealFoodResult(int Id);
}
