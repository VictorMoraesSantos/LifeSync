using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.Features.Diary.Queries.GetAll
{
    public record GetDiariesQuery : IQuery<GetDiariesResult>;
    public record GetDiariesResult(IEnumerable<DiaryDTO> Diaries);
}
