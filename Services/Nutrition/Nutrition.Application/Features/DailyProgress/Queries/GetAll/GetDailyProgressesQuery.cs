using MediatR;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetAll
{
    public record GetDailyProgressesQuery() : IRequest<GetDailyProgressesResult>;
    public record GetDailyProgressesResult(IEnumerable<DailyProgressDTO> DailyProgresses);
}
