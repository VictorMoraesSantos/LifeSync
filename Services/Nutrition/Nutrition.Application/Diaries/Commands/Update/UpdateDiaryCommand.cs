using MediatR;

namespace Nutrition.Application.Diaries.Commands.UpdateDiary
{
    public record UpdateDiaryCommand(int Id, DateOnly Date) : IRequest<UpdateDiaryCommandResult>;
    public record UpdateDiaryCommandResult(bool IsSuccess);
}
