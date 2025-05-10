using MediatR;

namespace Nutrition.Application.UseCases.Diaries.Commands.Update
{
    public record UpdateDiaryCommand(int Id, DateOnly Date) : IRequest<UpdateDiaryCommandResult>;
    public record UpdateDiaryCommandResult(bool IsSuccess);
}
