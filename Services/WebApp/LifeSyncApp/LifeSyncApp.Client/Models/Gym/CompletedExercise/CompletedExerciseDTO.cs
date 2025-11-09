namespace LifeSyncApp.Client.Models.Gym.CompletedExercise
{
    public record CompletedExerciseDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int TrainingSessionId,
        int RoutineExerciseId,
        SetCountDTO SetsCompleted,
        RepetitionCountDTO RepetitionsCompleted,
        WeightDTO? WeightUsed,
        string? Notes);
}