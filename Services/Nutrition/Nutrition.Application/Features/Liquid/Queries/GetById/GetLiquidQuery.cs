using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.Features.Liquid.Queries.GetById
{
    public record GetLiquidQuery(int Id) : IQuery<GetLiquidResult>;
    public record GetLiquidResult(LiquidDTO Liquid);
}
