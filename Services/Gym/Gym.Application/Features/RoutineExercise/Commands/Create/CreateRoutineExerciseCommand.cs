using BuildingBlocks.CQRS.Commands;
using Gym.Domain.ValueObjects;

namespace Gym.Application.Features.RoutineExercise.Commands.Create
{
    public record CreateRoutineExerciseCommand(
        int RoutineId,
        int ExerciseId,
        SetCount Sets,
        RepetitionCount Repetitions,
        RestTime RestBetweenSets,
        Weight? RecommendedWeight,
        string? Instructions) : ICommand<CreateRoutineExerciseResult>;

    public record CreateRoutineExerciseResult(int Id);
}
