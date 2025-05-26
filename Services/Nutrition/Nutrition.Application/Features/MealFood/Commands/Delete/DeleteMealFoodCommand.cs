using MediatR;

namespace Nutrition.Application.Features.MealFood.Commands.Delete
{
    public record DeleteMealFoodCommand(int Id) : IRequest<DeleteMealFoodResult>;
    public record DeleteMealFoodResult(bool IsSuccess);
}
