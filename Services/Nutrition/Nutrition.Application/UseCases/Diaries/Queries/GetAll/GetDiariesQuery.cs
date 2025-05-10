using MediatR;
using Nutrition.Application.DTOs.Diaries;

namespace Nutrition.Application.UseCases.Diaries.Queries.GetAll
{
    public record GetDiariesQuery : IRequest<GetDiariesQueryResult>;
    public record GetDiariesQueryResult(IEnumerable<DiaryDTO> Diaries);
}
