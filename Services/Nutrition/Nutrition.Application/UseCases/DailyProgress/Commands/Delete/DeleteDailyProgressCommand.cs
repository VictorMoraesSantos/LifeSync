using MediatR;

namespace Nutrition.Application.UseCases.DailyProgress.Commands.Delete
{
    public record DeleteDailyProgressCommand(int Id) : IRequest<DeleteDailyProgressResult>;
    public record DeleteDailyProgressResult(bool IsSuccess);
}
