using Core.Application.DTO;

namespace Gym.Application.DTOs.Routine
{
    public record RoutineDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string Description)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
