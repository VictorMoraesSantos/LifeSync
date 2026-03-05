using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.LiquidType.Commands.Update
{
    public record UpdateLiquidTypeCommand(int Id, string Name) : ICommand<UpdateLiquidTypeResult>;
    public record UpdateLiquidTypeResult(bool IsSuccess);
}
