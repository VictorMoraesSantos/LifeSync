namespace LifeSyncApp.Client.Models.Gym.RoutineExercise
{
    public record UpdateRoutineExerciseDTO(
        int Id,
        int RoutineId,
        int ExerciseId,
        SetCountDTO Sets,
        RepetitionCountDTO Repetitions,
        RestTimeDTO RestBetweenSets,
        WeightDTO? RecommendedWeight,
        string? Instructions);
}