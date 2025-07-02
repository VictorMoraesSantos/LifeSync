using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.DailyProgress.Commands.Delete
{
    public record DeleteDailyProgressCommand(int Id) : ICommand<DeleteDailyProgressResult>;
    public record DeleteDailyProgressResult(bool IsSuccess);
}
