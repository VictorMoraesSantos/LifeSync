using BuildingBlocks.CQRS.Commands;

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
