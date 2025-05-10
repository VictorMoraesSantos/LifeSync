using MediatR;

namespace Nutrition.Application.UseCases.Diaries.Commands.Create
{
    public record CreateDiaryCommand(int userId, DateOnly date) : IRequest<CreateDiaryCommandResult>;
    public record CreateDiaryCommandResult(bool IsSuccess);
}
