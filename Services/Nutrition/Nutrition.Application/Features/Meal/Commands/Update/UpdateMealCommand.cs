using MediatR;

namespace Nutrition.Application.Features.Meal.Commands.Update
{
    public record UpdateMealCommand(int Id, string Name, string Description) : IRequest<UpdateMealResult>;
    public record UpdateMealResult(bool IsSuccess);
}
