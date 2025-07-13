using Gym.Application.DTOs.Routine;
using Gym.Domain.Entities;

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
                entity.UserId,
                entity.Description,
                entity.Name);

            return dto;
        }

        public static Routine ToEntity(this CreateRoutineDTO dto)
        {
            var entity = new Routine(
                dto.UserId,
                dto.Description,
                dto.Name);

            return entity;
        }
    }
}
