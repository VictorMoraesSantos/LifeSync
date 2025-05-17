using MediatR;

namespace Nutrition.Application.UseCases.Diary.Commands.Update
{
    public record UpdateDiaryCommand(int Id, DateOnly Date) : IRequest<UpdateDiaryResult>;
    public record UpdateDiaryResult(bool IsSuccess);
}
