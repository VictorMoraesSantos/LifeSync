using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.MealFood.Commands.Update
{
    public record UpdateMealFoodCommand(
        int Id,
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
        : ICommand<UpdateMealFoodResult>;
    public record UpdateMealFoodResult(bool IsSuccess);
}
