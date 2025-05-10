using MediatR;

namespace Nutrition.Application.UseCases.Meals.Commands.Create
{
    public record CreateMealCommand(int DiaryId, string Name, string Description) : IRequest<CreateMealResponse>;
    public record CreateMealResponse(bool IsSuccess);
}
