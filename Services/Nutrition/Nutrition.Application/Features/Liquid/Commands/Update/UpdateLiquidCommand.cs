
using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Liquid.Commands.Update
{
    public record UpdateLiquidCommand(int Id, string Name, int QuantityMl, int CaloriesPerMl) : ICommand<UpdateLiquidResult>;
    public record UpdateLiquidResult(bool IsSuccess);
}
