using Nutrition.Application.DTOs.LiquidType;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class LiquidTypeMapper
    {
        public static LiquidTypeDTO ToDTO(this LiquidType entity)
        {
            var dto = new LiquidTypeDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name);

            return dto;
        }

        public static LiquidType ToEntity(this CreateLiquidTypeDTO dto)
        {
            var liquidType = new LiquidType(dto.Name);

            return liquidType;
        }
    }
}
