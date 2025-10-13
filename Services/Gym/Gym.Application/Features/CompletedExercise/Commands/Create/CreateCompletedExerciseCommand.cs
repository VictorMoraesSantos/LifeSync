using BuildingBlocks.CQRS.Commands;
using Gym.Domain.ValueObjects;

namespace Gym.Application.Features.CompletedExercise.Commands.Create
{
    public record CreateCompletedExerciseCommand(
        int TrainingSessionId,
        int ExerciseId,
        int RoutineExerciseId,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        Weight? WeightUsed,
        string? Notes) : ICommand<CreateCompletedExerciseResult>;
    public record CreateCompletedExerciseResult(int Id);
}
