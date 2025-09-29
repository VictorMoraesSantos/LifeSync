
using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Diary.Commands.Create
{
    public record CreateDiaryCommand(int UserId, DateOnly Date) : ICommand<CreateDiaryResult>;
    public record CreateDiaryResult(int Id);
}
