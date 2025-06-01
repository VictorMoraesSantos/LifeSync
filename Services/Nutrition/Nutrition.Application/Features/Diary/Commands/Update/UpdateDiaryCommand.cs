
using BuildingBlocks.CQRS.Request;

namespace Nutrition.Application.Features.Diary.Commands.Update
{
    public record UpdateDiaryCommand(int Id, DateOnly Date) : IRequest<UpdateDiaryResult>;
    public record UpdateDiaryResult(bool IsSuccess);
}
