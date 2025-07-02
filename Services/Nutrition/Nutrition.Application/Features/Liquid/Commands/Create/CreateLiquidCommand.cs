
using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Liquid.Commands.Create
{
    public record CreateLiquidCommand(int DiaryId, string Name, int QuantityMl, int CaloriesPerMl) : ICommand<CreateLiquidResult>;
    public record CreateLiquidResult(int Id);
}
