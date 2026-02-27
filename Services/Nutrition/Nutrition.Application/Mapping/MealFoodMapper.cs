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
                entity.MealId,
                entity.Food.Id,
                entity.Food.Name,
                entity.Food.Calories,
                entity.Food.Protein,
                entity.Food.Lipids,
                entity.Food.Carbohydrates,
                entity.Food.Calcium,
                entity.Food.Magnesium,
                entity.Food.Iron,
                entity.Food.Sodium,
                entity.Food.Potassium,
                entity.Quantity,
                entity.TotalCalories);

            return dto;
        }

        public static MealFood ToEntity(this CreateMealFoodDTO dto)
        {
            var entity = new MealFood(
                dto.MealId,
                dto.FoodId,
                dto.Quantity);

            return entity;
        }
    }
}
