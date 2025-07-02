using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.Features.Diary.Queries.GetByUser
{
    public record GetAllDiariesByUserIdQuery(int UserId) : IQuery<GetAllDiariesByUserIdResult>;
    public record GetAllDiariesByUserIdResult(IEnumerable<DiaryDTO> Diaries);
}
