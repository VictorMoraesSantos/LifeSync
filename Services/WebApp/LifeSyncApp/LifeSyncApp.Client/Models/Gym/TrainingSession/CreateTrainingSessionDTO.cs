namespace LifeSyncApp.Client.Models.Gym.TrainingSession
{
    public record CreateTrainingSessionDTO(int UserId, int RoutineId, DateTime StartTime, DateTime EndTime);
}