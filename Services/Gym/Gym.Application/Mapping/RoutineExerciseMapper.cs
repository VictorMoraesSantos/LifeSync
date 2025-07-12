using Gym.Application.DTOs.RoutineExercise;
using Gym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Mapping
{
    public static class RoutineExerciseMapper
    {
        public static RoutineExerciseDTO ToDTO(this RoutineExercise entity)
        {
            var dto = new RoutineExerciseDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.RoutineId,
                entity.ExerciseId,
                entity.Sets,
                entity.Repetitions,
                entity.RestBetweenSets,
                entity.RecommendedWeight,
                entity.Instructions);

            return dto;
        }

        public static RoutineExercise ToEntity(this CreateRoutineExerciseDTO dto)
        {
            var entity = new RoutineExercise(
                dto.RoutineId,
                dto.ExerciseId,
                dto.Sets,
                dto.Repetitions,
                dto.RestBetweenSets,
                dto.RecommendedWeight,
                dto.Instructions);
         
            return entity;
        }
    }
}
