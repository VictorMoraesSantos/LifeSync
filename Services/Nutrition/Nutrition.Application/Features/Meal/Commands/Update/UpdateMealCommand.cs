
using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Meal.Commands.Update
{
    public record UpdateMealCommand(int Id, string Name, string Description) : ICommand<UpdateMealResult>;
    public record UpdateMealResult(bool IsSuccess);
}
