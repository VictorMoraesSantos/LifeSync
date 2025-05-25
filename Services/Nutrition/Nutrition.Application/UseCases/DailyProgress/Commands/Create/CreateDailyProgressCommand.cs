using MediatR;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.UseCases.DailyProgress.Commands.Create
{
    public record CreateDailyProgressCommand(
        int UserId,
        DateOnly Date,
        int? CaloriesConsumed,
        int? LiquidsConsumedMl)
        : IRequest<CreateDailyProgressResult>;
    public record CreateDailyProgressResult(int Id);
}
