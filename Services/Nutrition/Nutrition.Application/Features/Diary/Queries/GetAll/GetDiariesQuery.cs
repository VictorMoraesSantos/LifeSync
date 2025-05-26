using MediatR;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.Features.Diary.Queries.GetAll
{
    public record GetDiariesQuery : IRequest<GetDiariesResult>;
    public record GetDiariesResult(IEnumerable<DiaryDTO> Diaries);
}
