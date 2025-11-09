namespace LifeSyncApp.Client.Models.Gym.TrainingSession
{
    public record TrainingSessionDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int UserId,
        int RoutineId,
        DateTime StartTime,
        DateTime? EndTime,
        string? Notes);
}