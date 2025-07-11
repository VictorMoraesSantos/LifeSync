using Core.Application.DTO;
using Gym.Domain.Enums;

namespace Gym.Application.DTOs.Exercise
{
    public record class ExerciseDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string Description,
        MuscleGroup MuscleGroup,
        ExerciseType Type,
        EquipmentType? EquipmentType)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
