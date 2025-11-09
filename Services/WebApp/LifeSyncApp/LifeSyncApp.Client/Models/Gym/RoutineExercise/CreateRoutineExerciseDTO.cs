namespace LifeSyncApp.Client.Models.Gym.RoutineExercise
{
    public record CreateRoutineExerciseDTO(
        int RoutineId,
        int ExerciseId,
        SetCountDTO Sets,
        RepetitionCountDTO Repetitions,
        RestTimeDTO RestBetweenSets,
        WeightDTO? RecommendedWeight,
        string? Instructions);
}