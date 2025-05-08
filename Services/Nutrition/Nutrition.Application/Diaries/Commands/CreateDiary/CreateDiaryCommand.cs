using MediatR;

namespace Nutrition.Application.Diaries.Commands.CreateDiary
{
    public record CreateDiaryCommand(int userId, DateOnly date) : IRequest<CreateDiaryCommandResult>;
    public record CreateDiaryCommandResult(bool IsSuccess);
}
