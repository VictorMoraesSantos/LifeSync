using MediatR;

namespace Nutrition.Application.UseCases.Meal.Commands.Update
{
    public record UpdateMealCommand(int Id, string Name, string Description) : IRequest<UpdateMealResult>;
    public record UpdateMealResult(bool IsSuccess);
}
