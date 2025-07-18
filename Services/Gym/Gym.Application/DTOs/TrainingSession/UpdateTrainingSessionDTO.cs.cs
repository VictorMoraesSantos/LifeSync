namespace Gym.Application.DTOs.TrainingSession
{
    public record UpdateTrainingSessionDTO(
        int Id,
        int RoutineId,
        DateTime StartTime,
        DateTime EndTime,
        string Notes);
}
