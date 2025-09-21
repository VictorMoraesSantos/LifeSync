using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.Features.Liquid.Queries.Get
{
    public record GetLiquidQuery(int Id) : IQuery<GetLiquidResult>;
    public record GetLiquidResult(LiquidDTO Liquid);
}
