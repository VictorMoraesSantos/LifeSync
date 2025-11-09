namespace LifeSyncApp.Client.Models.Gym.RoutineExercise
{
    public record RoutineExerciseDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int RoutineId,
        int ExerciseId,
        SetCountDTO Sets,
        RepetitionCountDTO Repetitions,
        RestTimeDTO RestBetweenSets,
        WeightDTO? RecommendedWeight,
        string? Instructions);

    public record SetCountDTO(int Value);
    public record RepetitionCountDTO(int Value);
    public record RestTimeDTO(int Value);
    public record WeightDTO(int Value, WeightUnitDTO Unit);
}