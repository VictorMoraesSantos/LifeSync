using MediatR;

namespace Nutrition.Application.UseCases.Diary.Commands.Delete
{
    public record DeleteDiaryCommand(int Id) : IRequest<DeleteDiaryResult>;
    public record DeleteDiaryResult(bool IsSuccess);
}
