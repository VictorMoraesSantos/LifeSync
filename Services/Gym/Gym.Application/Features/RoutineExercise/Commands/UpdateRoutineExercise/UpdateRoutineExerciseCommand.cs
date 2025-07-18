using BuildingBlocks.CQRS.Commands;
using Gym.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Gym.Application.Features.RoutineExercise.Commands.UpdateRoutineExercise
{
    public record UpdateRoutineExerciseCommand(
        int Id,
        SetCount Sets,
        RepetitionCount Repetitions,
        RestTime RestBetweenSets,
        Weight? RecommendedWeight,
        string? Instructions)
        : ICommand<UpdateRoutineExerciseCommandResponse>;
    public record UpdateRoutineExerciseCommandResponse(bool IsSeccess);
}
