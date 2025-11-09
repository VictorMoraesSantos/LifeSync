namespace LifeSyncApp.Client.Models.Gym.CompletedExercise
{
    public record UpdateCompletedExerciseDTO(
        int Id,
        SetCountDTO SetsCompleted,
        RepetitionCountDTO RepetitionsCompleted,
        RestTimeDTO RestBetweenSets,
        WeightDTO? WeightUsed,
        string? Notes);
}