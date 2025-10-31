using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.Features.Liquid.Queries.GetByFilter
{
    public record GetByFilterQuery(LiquidQueryFilterDTO Filter) : IQuery<GetByFilterResult>;
    public record GetByFilterResult(IEnumerable<LiquidDTO> Items, PaginationData Pagination);
}
