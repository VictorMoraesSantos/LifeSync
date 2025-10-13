using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.CompletedExercise.Commands.Delete
{
    public record DeleteCompletedExerciseCommand(int Id) : ICommand<DeleteCompletedExerciseResult>;
    public record DeleteCompletedExerciseResult(bool IsSuccess);
}
