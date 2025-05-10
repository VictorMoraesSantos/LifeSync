using MediatR;
using Nutrition.Application.DTOs.Diaries;

namespace Nutrition.Application.Diaries.Queries.GetAllDiariesByUserId
{
    public record GetAllDiariesByUserIdQuery(int UserId) : IRequest<GetAllDiariesByUserIdResult>;
    public record GetAllDiariesByUserIdResult(IEnumerable<DiaryDTO> Diaries);
}
