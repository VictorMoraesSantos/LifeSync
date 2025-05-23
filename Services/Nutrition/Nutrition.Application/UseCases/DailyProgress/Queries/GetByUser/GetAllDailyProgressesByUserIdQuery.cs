using MediatR;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.UseCases.DailyProgress.Queries.GetByUser
{
    public record GetAllDailyProgressesByUserIdQuery(int UserId) : IRequest<GetAllDailyProgressesByUserIdResult>;
    public record GetAllDailyProgressesByUserIdResult(IEnumerable<DailyProgressDTO> DailyProgresses);
}
