using MediatR;

namespace Nutrition.Application.UseCases.Meal.Commands.Delete
{
    public record DeleteMealCommand(int Id) : IRequest<DeleteMealResult>;
    public record DeleteMealResult(bool IsSuccess);
}
