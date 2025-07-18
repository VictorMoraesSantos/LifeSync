using Gym.Domain.ValueObjects;

namespace Gym.Application.DTOs.CompletedExercise
{
    public record UpdateCompletedExerciseDTO(
        int Id,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        RestTime RestBetweenSets,
        Weight? WeightUsed,
        string? Notes);
}
