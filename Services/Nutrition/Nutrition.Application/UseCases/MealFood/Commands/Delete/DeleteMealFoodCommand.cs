using MediatR;

namespace Nutrition.Application.UseCases.MealFood.Commands.Delete
{
    public record DeleteMealFoodCommand(int Id) : IRequest<DeleteMealFoodResult>;
    public record DeleteMealFoodResult(bool IsSuccess);
}
