using MediatR;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.UseCases.DailyProgress.Queries.Get
{
    public record GetDailyProgressQuery(int Id) : IRequest<GetDailyProgressResult>;
    public record GetDailyProgressResult(DailyProgressDTO DailyProgress);
}
