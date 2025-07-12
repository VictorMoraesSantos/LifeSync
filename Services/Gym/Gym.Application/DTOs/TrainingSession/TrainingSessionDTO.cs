using Core.Application.DTO;

namespace Gym.Application.DTOs.TrainingSession
{
    public record TrainingSessionDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int UserId,
        int RoutineId,
        DateTime StartTime,
        DateTime? EndTime,
        string? Notes)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
