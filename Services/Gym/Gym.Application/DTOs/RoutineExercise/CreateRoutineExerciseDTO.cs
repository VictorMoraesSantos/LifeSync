using Gym.Domain.ValueObjects;

namespace Gym.Application.DTOs.RoutineExercise
{
    public record CreateRoutineExerciseDTO(
        int RoutineId,
        int ExerciseId,
        SetCount Sets,
        RepetitionCount Repetitions,
        RestTime RestBetweenSets,
        Weight? RecommendedWeight,
        string? Instructions);
}
