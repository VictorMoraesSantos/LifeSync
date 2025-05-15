using Nutrition.Application.DTOs.Liquid;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class LiquidMapper
    {
        public static LiquidDTO ToDTO(this Liquid entity)
        {
            LiquidDTO dto = new(
                entity.Id,
                entity.DiaryId,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.QuantityMl,
                entity.CaloriesPerMl,
                entity.TotalCalories);

            return dto;
        }

        public static Liquid ToEntity(this CreateLiquidDTO dto)
        {
            Liquid liquid = new(
                dto.DiaryId,
                dto.Name,
                dto.QuantityMl,
                dto.CaloriesPerMl);

            return liquid;
        }
    }
}
