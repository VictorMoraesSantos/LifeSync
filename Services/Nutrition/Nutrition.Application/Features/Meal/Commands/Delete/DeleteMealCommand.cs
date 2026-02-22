using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.Meal.Commands.Delete
{
    public record DeleteMealCommand(int Id) : ICommand<DeleteMealResult>;
    public record DeleteMealResult(bool IsSuccess);
}
