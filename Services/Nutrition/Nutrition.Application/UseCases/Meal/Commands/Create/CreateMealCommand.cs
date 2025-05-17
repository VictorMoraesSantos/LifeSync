using MediatR;

namespace Nutrition.Application.UseCases.Meal.Commands.Create
{
    public record CreateMealCommand(int DiaryId, string Name, string Description) : IRequest<CreateMealResult>;
    public record CreateMealResult(bool IsSuccess);
}
