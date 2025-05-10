using MediatR;
using Nutrition.Application.DTOs.Diaries;

namespace Nutrition.Application.Diaries.Queries.GetDiaries
{
    public record GetDiariesQuery : IRequest<GetDiariesQueryResult>;
    public record GetDiariesQueryResult(IEnumerable<DiaryDTO> Diaries);
}
