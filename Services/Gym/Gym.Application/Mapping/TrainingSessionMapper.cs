using Gym.Application.DTOs.TrainingSession;
using Gym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Mapping
{
    public static class TrainingSessionMapper
    {
        public static TrainingSessionDTO ToDTO(this TrainingSession entity)
        {
            var dto = new TrainingSessionDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.UserId,
                entity.RoutineId,
                entity.StartTime,
                entity.EndTime,
                entity.Notes);

            return dto;
        }

        public static TrainingSession ToEntity(this CreateTrainingSessionDTO dto)
        {
            var entity = new TrainingSession(
                dto.UserId,
                dto.RoutineId,
                dto.StartTime,
                dto.EndTime);

            return entity;
        }
    }
}
