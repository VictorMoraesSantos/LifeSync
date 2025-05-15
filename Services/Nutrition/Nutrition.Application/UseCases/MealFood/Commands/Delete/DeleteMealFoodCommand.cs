using MediatR;

namespace Nutrition.Application.UseCases.MealFood.Commands.Delete
{
    public record DeleteMealFoodCommand(int Id) : IRequest<DeleteMealFoodResponse>;
    public record DeleteMealFoodResponse(bool IsSuccess);
}
