using Gym.Domain.ValueObjects;

namespace Gym.Application.DTOs.CompletedExercise
{
    public record UpdateCompletedExerciseDTO(
        int Id,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        Weight? WeightUsed,
        string? Notes);
}
