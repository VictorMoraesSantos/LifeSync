using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetByFilter
{
    public record GetDailyProgressByFilterQuery(DailyProgressQueryFilterDTO Filter) : IQuery<GetDailyProgressByFilterResult>;
    public record GetDailyProgressByFilterResult(IEnumerable<DailyProgressDTO> Items, PaginationData Pagination);
}
