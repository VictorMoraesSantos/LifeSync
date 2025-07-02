using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.Features.Diary.Queries.Get
{
    public record GetDiaryQuery(int Id) : IQuery<GetDiaryResult>;
    public record GetDiaryResult(DiaryDTO Diary);
}
