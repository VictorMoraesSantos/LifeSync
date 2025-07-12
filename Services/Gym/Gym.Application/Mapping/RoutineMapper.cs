using Gym.Application.DTOs.Routine;
using Gym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Mapping
{
    public static class RoutineMapper
    {
        public static RoutineDTO ToDTO(this RoutineDTO entity)
        {
            var dto = new RoutineDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.Description,
                entity.UserId);

            return dto;
        }

        public static Routine ToEntity(this CreateRoutineDTO dto)
        {
            var entity = new Routine(
                dto.Name,
                dto.Description,
                dto.UserId);

            return entity;
        }
    }
}
