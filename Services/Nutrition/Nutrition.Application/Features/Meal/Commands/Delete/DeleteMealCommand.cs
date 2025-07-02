using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Meal.Commands.Delete
{
    public record DeleteMealCommand(int Id) : ICommand<DeleteMealResult>;
    public record DeleteMealResult(bool IsSuccess);
}
