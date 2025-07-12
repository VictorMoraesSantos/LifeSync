using Gym.Domain.ValueObjects;

namespace Gym.Application.DTOs.CompletedExercise
{
    public record CreateCompletedExerciseDTO(
        int TrainingSessionId,
        int ExerciseId,
        int RoutineExerciseId,
        SetCount SetsCompleted,
        RepetitionCount RepetitionsCompleted,
        Weight? WeightUsed,
        string? Notes);
}
