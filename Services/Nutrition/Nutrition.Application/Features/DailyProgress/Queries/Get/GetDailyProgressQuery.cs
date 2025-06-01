using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Queries.Get
{
    public record GetDailyProgressQuery(int Id) : IRequest<GetDailyProgressResult>;
    public record GetDailyProgressResult(DailyProgressDTO DailyProgress);
}
