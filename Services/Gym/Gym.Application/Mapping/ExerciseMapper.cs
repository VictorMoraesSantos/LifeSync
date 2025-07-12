using Gym.Application.DTOs.CompletedExercise;
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

        public static CompletedExercise ToEntity(this CreateCompletedExerciseDTO dto)
        {
            var entity = new CompletedExercise(
                dto.TrainingSessionId,
                dto.ExerciseId,
                dto.RoutineExerciseId,
                dto.SetsCompleted,
                dto.RepetitionsCompleted,
                dto.WeightUsed,
                dto.Notes);

            return entity;
        }
    }
}
