using MediatR;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.UseCases.Diary.Queries.Get
{
    public record GetDiaryQuery(int Id) : IRequest<GetDiaryResult>;
    public record GetDiaryResult(DiaryDTO Diary);
}
