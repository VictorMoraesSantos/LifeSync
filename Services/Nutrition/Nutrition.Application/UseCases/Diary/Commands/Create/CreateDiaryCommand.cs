using MediatR;

namespace Nutrition.Application.UseCases.Diary.Commands.Create
{
    public record CreateDiaryCommand(int userId, DateOnly date) : IRequest<CreateDiaryResult>;
    public record CreateDiaryResult(int Id);
}
