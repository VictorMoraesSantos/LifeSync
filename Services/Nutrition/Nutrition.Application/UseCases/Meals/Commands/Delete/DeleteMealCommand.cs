using MediatR;

namespace Nutrition.Application.UseCases.Meals.Commands.Delete
{
    public record DeleteMealCommand(int Id) : IRequest<DeleteMealResponse>;
    public record DeleteMealResponse(bool IsSuccess);
}
