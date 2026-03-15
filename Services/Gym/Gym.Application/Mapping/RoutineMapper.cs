using Gym.Application.DTOs.Routine;
using Gym.Domain.Entities;

namespace Gym.Application.Mapping
{
    public static class RoutineMapper
    {
        public static RoutineDTO ToDTO(this Routine entity)
        {
            var dto = new RoutineDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.Description);

            return dto;
        }

        public static Routine ToEntity(this CreateRoutineDTO dto)
        {
            var entity = new Routine(
                dto.Name,
                dto.Description);

            return entity;
        }
    }
}
