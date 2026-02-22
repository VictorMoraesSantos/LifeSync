using Nutrition.Application.DTOs.MealFood;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class MealFoodMapper
    {
        public static MealFoodDTO ToDTO(this MealFood entity)
        {
            var dto = new MealFoodDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Code,
                entity.Name,
                entity.Calories,
                entity.Protein,
                entity.Lipids,
                entity.Carbohydrates,
                entity.Calcium,
                entity.Magnesium,
                entity.Iron,
                entity.Sodium,
                entity.Potassium,
                entity.Quantity,
                entity.TotalCalories);

            return dto;
        }

        public static MealFood ToEntity(this CreateMealFoodDTO dto)
        {
            var entity = new MealFood(
                dto.Code,
                dto.Name,
                dto.Calories,
                dto.Protein,
                dto.Lipids,
                dto.Carbohydrates,
                dto.Calcium,
                dto.Magnesium,
                dto.Iron,
                dto.Sodium,
                dto.Potassium,
                dto.Quantity);
            
            return entity;
        }
    }
}
