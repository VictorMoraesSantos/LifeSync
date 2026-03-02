using Nutrition.Application.DTOs.Food;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class FoodMapper
    {
        public static FoodDTO ToDTO(this Food entity)
        {
            var dto = new FoodDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.Calories,
                entity.Protein,
                entity.Lipids,
                entity.Carbohydrates,
                entity.Calcium,
                entity.Magnesium,
                entity.Iron,
                entity.Sodium,
                entity.Potassium);
            return dto;
        }
    }
}
