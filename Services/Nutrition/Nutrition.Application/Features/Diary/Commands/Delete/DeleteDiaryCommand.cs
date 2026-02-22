using BuildingBlocks.CQRS.Requests.Commands;

namespace Nutrition.Application.Features.Diary.Commands.Delete
{
    public record DeleteDiaryCommand(int Id) : ICommand<DeleteDiaryResult>;
    public record DeleteDiaryResult(bool IsSuccess);
}
