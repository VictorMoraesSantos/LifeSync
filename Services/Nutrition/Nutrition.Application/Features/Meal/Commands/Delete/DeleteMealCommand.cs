using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.Meal.Commands.Delete
{
    public record DeleteMealCommand(int Id) : IRequest<DeleteMealResult>;
    public record DeleteMealResult(bool IsSuccess);
}
