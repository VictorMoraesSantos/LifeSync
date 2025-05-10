using MediatR;

namespace Nutrition.Application.UseCases.Meals.Commands.Update
{
    public record UpdateMealCommand(int Id, string Name, string Description) : IRequest<UpdateMealResponse>;
    public record UpdateMealResponse(bool IsSuccess);
}
