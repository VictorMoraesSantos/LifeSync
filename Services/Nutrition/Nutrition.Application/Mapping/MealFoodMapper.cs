using Nutrition.Application.DTOs.MealFood;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class MealFoodMapper
    {
        public static MealFoodDTO ToDTO(this MealFood entity)
        {
            MealFoodDTO dto = new(
                entity.Id,
                entity.MealId,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.Quantity,
                entity.CaloriesPerUnit,
                entity.TotalCalories);
            
            return dto;
        }

        public static MealFood ToEntity(this CreateMealFoodDTO dto)
        {
            return new MealFood(
                dto.Name,
                dto.QuantityInGrams,
                dto.CaloriesPerUnit);
        }
    }
}
