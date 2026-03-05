using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.LiquidType.Commands.Create
{
    public record CreateLiquidTypeCommand(string Name) : ICommand<CreateLiquidTypeResult>;
    public record CreateLiquidTypeResult(int Id);
}
