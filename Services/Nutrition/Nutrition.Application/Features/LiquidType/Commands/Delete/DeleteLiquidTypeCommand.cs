using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.LiquidType.Commands.Delete
{
    public record DeleteLiquidTypeCommand(int Id) : ICommand<DeleteLiquidTypeResult>;
    public record DeleteLiquidTypeResult(bool IsSuccess);
}
