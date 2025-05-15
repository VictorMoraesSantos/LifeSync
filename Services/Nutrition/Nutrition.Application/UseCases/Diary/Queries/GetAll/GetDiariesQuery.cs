using MediatR;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.UseCases.Diary.Queries.GetAll
{
    public record GetDiariesQuery : IRequest<GetDiariesQueryResult>;
    public record GetDiariesQueryResult(IEnumerable<DiaryDTO> Diaries);
}
