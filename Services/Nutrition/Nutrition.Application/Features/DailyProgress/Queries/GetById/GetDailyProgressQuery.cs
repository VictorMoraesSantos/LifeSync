using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Queries.Get
{
    public record GetDailyProgressQuery(int Id) : IQuery<GetDailyProgressResult>;
    public record GetDailyProgressResult(DailyProgressDTO DailyProgress);
}
