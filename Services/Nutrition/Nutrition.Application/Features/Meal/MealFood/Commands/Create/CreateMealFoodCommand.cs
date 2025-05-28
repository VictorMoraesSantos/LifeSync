using MediatR;

namespace Nutrition.Application.Features.Meal.MealFood.Commands.Create
{
    public record CreateMealFoodCommand(
        int MealId,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit)
        : IRequest<CreateMealFoodResult>;
    public record CreateMealFoodResult(int Id);
}
