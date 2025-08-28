using BuildingBlocks.CQRS.Commands;
using Gym.Domain.ValueObjects;

namespace Gym.Application.Features.RoutineExercise.Commands.UpdateRoutineExercise
{
    public record UpdateRoutineExerciseCommand(
        int Id,
        SetCount Sets,
        RepetitionCount Repetitions,
        RestTime RestBetweenSets,
        Weight? RecommendedWeight,
        string? Instructions)
        : ICommand<UpdateRoutineExerciseResult>;
    public record UpdateRoutineExerciseResult(bool IsSeccess);
}
