using BuildingBlocks.CQRS.Commands;
using Gym.Domain.ValueObjects;

namespace Gym.Application.Features.CompletedExercise.Commands.Update
{
    public record UpdateCompletedExerciseCommand(
        int Id,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        RestTime RestBetweenSets,
        Weight? WeightUsed,
        string? Notes) : ICommand<UpdateCompletedExerciseResult>;
    public record UpdateCompletedExerciseResult(bool IsSuccess);
}
