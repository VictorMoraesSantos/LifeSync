using MediatR;

namespace Nutrition.Application.Features.Diary.Commands.Create
{
    public record CreateDiaryCommand(int userId, DateOnly date) : IRequest<CreateDiaryResult>;
    public record CreateDiaryResult(int Id);
}
