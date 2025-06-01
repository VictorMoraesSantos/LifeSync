using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.DailyProgress.Commands.Create
{
    public record CreateDailyProgressCommand(
        int UserId,
        DateOnly Date,
        int? CaloriesConsumed,
        int? LiquidsConsumedMl)
        : IRequest<CreateDailyProgressResult>;
    public record CreateDailyProgressResult(int Id);
}
