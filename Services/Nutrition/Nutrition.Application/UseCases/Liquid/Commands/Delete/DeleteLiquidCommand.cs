using MediatR;

namespace Nutrition.Application.UseCases.Liquid.Commands.Delete
{
    public record DeleteLiquidCommand(int Id) : IRequest<DeleteLiquidResult>;
    public record DeleteLiquidResult(bool IsSuccess);
}
