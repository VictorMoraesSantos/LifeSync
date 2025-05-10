using MediatR;

namespace Nutrition.Application.UseCases.MealFoods.Commands.Update
{
    public record UpdateMealFoodCommand(
        int Id,
        string Name,
        int QuantityInGrams,
        int CaloriesPerUnit)
        : IRequest<UpdateMealFoodResponse>;
    public record UpdateMealFoodResponse(bool IsSuccess);
}
