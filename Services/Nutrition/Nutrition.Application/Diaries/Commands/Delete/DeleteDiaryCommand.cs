using MediatR;

namespace Nutrition.Application.Diaries.Commands.DeleteDiary
{
    public record DeleteDiaryCommand(int Id) : IRequest<DeleteDiaryCommandResult>;
    public record DeleteDiaryCommandResult(bool IsSuccess);
}
