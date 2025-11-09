namespace LifeSyncApp.Client.Models.Gym.CompletedExercise
{
    public record CreateCompletedExerciseDTO(
        int TrainingSessionId,
        int ExerciseId,
        int RoutineExerciseId,
        SetCountDTO SetsCompleted,
        RepetitionCountDTO RepetitionsCompleted,
        WeightDTO? WeightUsed,
        string? Notes);
}