using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.Exercise.Commands.Delete
{
    public record DeleteExerciseCommand(int Id) : ICommand<DeleteExerciseResult>;
    public record DeleteExerciseResult(bool IsSuccess);
}
