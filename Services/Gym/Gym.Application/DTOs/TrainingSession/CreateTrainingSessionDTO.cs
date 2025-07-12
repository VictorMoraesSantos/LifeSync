namespace Gym.Application.DTOs.TrainingSession
{
    public record CreateTrainingSessionDTO(
        int UserId,
        int RoutineId,
        DateTime StartTime,
        DateTime EndTime);
}
