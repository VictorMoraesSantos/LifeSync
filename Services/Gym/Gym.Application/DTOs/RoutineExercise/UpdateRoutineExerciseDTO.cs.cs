using Gym.Domain.ValueObjects;

namespace Gym.Application.DTOs.RoutineExercise
{
    public record UpdateRoutineExerciseDTO(
        int Id,
        SetCount Sets,
        RepetitionCount Repetitions,
        RestTime RestBetweenSets,
        Weight? RecommendedWeight,
        string? Instructions);
}
