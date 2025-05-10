using MediatR;
using Nutrition.Application.DTOs.Diaries;

namespace Nutrition.Application.UseCases.Diaries.Queries.Get
{
    public record GetDiaryQuery(int Id) : IRequest<GetDiaryQueryResult>;
    public record GetDiaryQueryResult(DiaryDTO Diary);
}
