using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.DailyProgress.Commands.Create
{
    public record CreateDailyProgressCommand(
        int UserId,
        DateOnly Date,
        int? CaloriesConsumed,
        int? LiquidsConsumedMl)
        : ICommand<CreateDailyProgressResult>;
    public record CreateDailyProgressResult(int Id);
}
