namespace Gym.Application.DTOs.TrainingSession
{
    public record UpdateTrainingSessionDTO(
        int Id,
        DateTime StartTime,
        DateTime EndTime,
        string? Notes);
}
