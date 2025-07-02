using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.DailyProgress.Commands.Delete
{
    public record DeleteDailyProgressCommand(int Id) : ICommand<DeleteDailyProgressResult>;
    public record DeleteDailyProgressResult(bool IsSuccess);
}
