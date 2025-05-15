using MediatR;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.UseCases.Diary.Queries.Get
{
    public record GetDiaryQuery(int Id) : IRequest<GetDiaryQueryResult>;
    public record GetDiaryQueryResult(DiaryDTO Diary);
}
