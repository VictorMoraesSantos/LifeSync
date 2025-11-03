using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.Features.Diary.Queries.GetByFilter
{
    public record GetDiaryByFilterQuery(DiaryQueryFilterDTO Filter) : IQuery<GetDiaryByFilterResult>;
    public record GetDiaryByFilterResult(IEnumerable<DiaryDTO> Items, PaginationData Pagination);
}
