using MediatR;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.Features.Diary.Queries.GetByUser
{
    public record GetAllDiariesByUserIdQuery(int UserId) : IRequest<GetAllDiariesByUserIdResult>;
    public record GetAllDiariesByUserIdResult(IEnumerable<DiaryDTO> Diaries);
}
