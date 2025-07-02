
using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Diary.Commands.Update
{
    public record UpdateDiaryCommand(int Id, DateOnly Date) : ICommand<UpdateDiaryResult>;
    public record UpdateDiaryResult(bool IsSuccess);
}
