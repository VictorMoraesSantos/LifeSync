
using BuildingBlocks.CQRS.Commands;

namespace Nutrition.Application.Features.Diary.Commands.Create
{
    public record CreateDiaryCommand(int userId, DateOnly date) : ICommand<CreateDiaryResult>;
    public record CreateDiaryResult(int Id);
}
