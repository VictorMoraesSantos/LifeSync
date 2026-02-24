using Nutrition.Application.DTOs.Liquid;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class LiquidMapper
    {
        public static LiquidDTO ToDTO(this Liquid entity)
        {
            var dto = new LiquidDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.DiaryId,
                entity.LiquidType.Name,
                entity.Quantity);

            return dto;
        }

        public static Liquid ToEntity(this CreateLiquidDTO dto)
        {
            var liquid = new Liquid(
                dto.DiaryId,
                dto.LiquidTypeId,
                dto.Quantity);

            return liquid;
        }
    }
}
