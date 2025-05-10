using MediatR;

namespace Nutrition.Application.UseCases.Diaries.Commands.Delete
{
    public record DeleteDiaryCommand(int Id) : IRequest<DeleteDiaryCommandResult>;
    public record DeleteDiaryCommandResult(bool IsSuccess);
}
