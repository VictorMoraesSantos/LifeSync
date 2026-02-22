
using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.MealFood.Commands.Create
{
    public record CreateMealFoodCommand(
        int MealId,
        int Code,
        string Name,
        int Calories,
        decimal Protein,
        decimal Lipids,
        decimal Carbohydrates,
        decimal Calcium,
        decimal Magnesium,
        decimal Iron,
        decimal Sodium,
        decimal Potassium,
        int Quantity)
        : ICommand<CreateMealFoodResult>;
    public record CreateMealFoodResult(int Id);
}
