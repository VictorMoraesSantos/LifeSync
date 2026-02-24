using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.Liquid.Commands.Create
{
    public record CreateLiquidCommand(int DiaryId, int LiquidTypeId, int Quantity) : ICommand<CreateLiquidResult>;
    public record CreateLiquidResult(int Id);
}
