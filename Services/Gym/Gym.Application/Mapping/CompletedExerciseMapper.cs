using Gym.Application.DTOs.CompletedExercise;
using Gym.Domain.Entities;

namespace Gym.Application.Mapping
{
    public static class CompletedExerciseMapper
    {
        public static CompletedExerciseDTO ToDTO(this CompletedExercise entity)
        {
            var dto = new CompletedExerciseDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.TrainingSessionId,
                entity.ExerciseId,
                entity.RoutineExerciseId,
                entity.SetsCompleted,
                entity.RepetitionsCompleted,
                entity.WeightUsed,
                entity.Notes);

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
