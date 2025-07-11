using Core.Application.DTO;

namespace Gym.Application.DTOs.Routine
{
    public record RoutineDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string Description,
        int UserId)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
