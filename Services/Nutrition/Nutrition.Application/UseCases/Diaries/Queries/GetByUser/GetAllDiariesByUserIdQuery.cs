using MediatR;
using Nutrition.Application.DTOs.Diaries;

namespace Nutrition.Application.UseCases.Diaries.Queries.GetByUser
{
    public record GetAllDiariesByUserIdQuery(int UserId) : IRequest<GetAllDiariesByUserIdResult>;
    public record GetAllDiariesByUserIdResult(IEnumerable<DiaryDTO> Diaries);
}
