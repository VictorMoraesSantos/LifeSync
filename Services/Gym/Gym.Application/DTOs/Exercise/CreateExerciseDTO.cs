using Gym.Domain.Enums;

namespace Gym.Application.DTOs.Exercise
{
    public record CreateExerciseDTO(
        string Name,
        string Description,
        MuscleGroup MuscleGroup,
        ExerciseType Type,
        EquipmentType? EquipmentType);
}
