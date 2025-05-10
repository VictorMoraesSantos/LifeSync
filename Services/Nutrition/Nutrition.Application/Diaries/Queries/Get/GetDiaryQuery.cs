using MediatR;
using Nutrition.Application.DTOs.Diaries;

namespace Nutrition.Application.Diaries.Queries.GetDiary
{
    public record GetDiaryQuery(int Id) : IRequest<GetDiaryQueryResult>;
    public record GetDiaryQueryResult(DiaryDTO Diary);
}
