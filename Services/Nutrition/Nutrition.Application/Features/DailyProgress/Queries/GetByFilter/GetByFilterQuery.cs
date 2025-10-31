using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetByFilter
{
    public record GetByFilterQuery(DailyProgressQueryFilterDTO Filter) : IQuery<GetByFilterResult>;
    public record GetByFilterResult(IEnumerable<DailyProgressDTO> Items, PaginationData Pagination);
}
