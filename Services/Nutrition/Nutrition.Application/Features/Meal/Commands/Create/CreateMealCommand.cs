using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.Meal.Commands.Create
{
    public record CreateMealCommand(int DiaryId, string Name, string Description) : ICommand<CreateMealResult>;
    public record CreateMealResult(bool IsSuccess);
}
