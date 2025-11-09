namespace LifeSyncApp.Client.Models.Gym.TrainingSession
{
    public record UpdateTrainingSessionDTO(int Id, int RoutineId, DateTime StartTime, DateTime EndTime, string Notes);
}