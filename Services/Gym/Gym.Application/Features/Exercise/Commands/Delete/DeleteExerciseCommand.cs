using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.Exercise.Commands.DeleteExerciseCommand
{
    public record DeleteExerciseCommand(int Id) : ICommand<DeleteExerciseResult>;
    public record DeleteExerciseResult(bool IsSuccess);
}
