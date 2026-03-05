using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.LiquidType;

namespace Nutrition.Application.Features.LiquidType.Queries.GetByFilter
{
    public record GetLiquidTypeByFilterQuery(LiquidTypeQueryFilterDTO Filter) : IQuery<GetLiquidTypeByFilterResult>;
    public record GetLiquidTypeByFilterResult(IEnumerable<LiquidTypeDTO> Items, PaginationData Pagination);
}
