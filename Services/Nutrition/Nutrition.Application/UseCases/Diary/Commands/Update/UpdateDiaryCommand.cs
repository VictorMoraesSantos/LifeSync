using MediatR;

namespace Nutrition.Application.UseCases.Diary.Commands.Update
{
    public record UpdateDiaryCommand(int Id, DateOnly Date) : IRequest<UpdateDiaryCommandResult>;
    public record UpdateDiaryCommandResult(bool IsSuccess);
}
