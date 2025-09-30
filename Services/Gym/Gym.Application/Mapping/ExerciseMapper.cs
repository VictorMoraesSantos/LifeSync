using Gym.Application.DTOs.Exercise;
using Gym.Domain.Entities;

namespace Gym.Application.Mapping
{
    public static class ExerciseMapper
    {
        public static ExerciseDTO ToDTO(this Exercise entity)
        {
            var dto = new ExerciseDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.Description,
                entity.MuscleGroup,
                entity.Type,
                entity.EquipmentType);

            return dto;
        }

        public static Exercise ToEntity(this CreateExerciseDTO dto)
        {
            var entity = new Exercise(
                dto.Name,
                dto.Description,
                dto.MuscleGroup,
                dto.ExerciseType,
                dto.EquipmentType);

            return entity;
        }
    }
}
