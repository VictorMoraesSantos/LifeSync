using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetByUser
{
    public record GetAllDailyProgressesByUserIdQuery(int UserId) : IQuery<GetAllDailyProgressesByUserIdResult>;
    public record GetAllDailyProgressesByUserIdResult(IEnumerable<DailyProgressDTO> DailyProgresses);
}
