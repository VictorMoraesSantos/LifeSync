using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.Features.Liquid.Queries.GetByFilter
{
    public record GetLiquidByFilterQuery(LiquidQueryFilterDTO Filter) : IQuery<GetLiquidByFilterResult>;
    public record GetLiquidByFilterResult(IEnumerable<LiquidDTO> Items, PaginationData Pagination);
}
