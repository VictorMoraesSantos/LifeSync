
using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Liquid.Commands.Delete
{
    public record DeleteLiquidCommand(int Id) : ICommand<DeleteLiquidResult>;
    public record DeleteLiquidResult(bool IsSuccess);
}
