using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.Liquid.Commands.Update
{
    public record UpdateLiquidCommand(int Id, int LiquidTypeId, int Quantity) : ICommand<UpdateLiquidResult>;
    public record UpdateLiquidResult(bool IsSuccess);
}
