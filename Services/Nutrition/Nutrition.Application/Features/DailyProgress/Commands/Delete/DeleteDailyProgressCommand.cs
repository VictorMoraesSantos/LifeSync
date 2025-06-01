using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.DailyProgress.Commands.Delete
{
    public record DeleteDailyProgressCommand(int Id) : IRequest<DeleteDailyProgressResult>;
    public record DeleteDailyProgressResult(bool IsSuccess);
}
