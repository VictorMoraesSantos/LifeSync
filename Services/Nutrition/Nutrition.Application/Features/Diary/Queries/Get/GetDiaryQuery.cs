using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.Features.Diary.Queries.Get
{
    public record GetDiaryQuery(int Id) : IRequest<GetDiaryResult>;
    public record GetDiaryResult(DiaryDTO Diary);
}
