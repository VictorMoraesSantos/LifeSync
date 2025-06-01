
using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.Liquid.Commands.Delete
{
    public record DeleteLiquidCommand(int Id) : IRequest<DeleteLiquidResult>;
    public record DeleteLiquidResult(bool IsSuccess);
}
