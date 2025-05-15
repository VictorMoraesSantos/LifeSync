using Nutrition.Application.DTOs.Meal;
using Nutrition.Domain.Entities;

namespace Nutrition.Application.Mapping
{
    public static class MealMapper
    {
        public static MealDTO ToDTO(this Meal entity)
        {
            return new MealDTO(
                entity.Id,
                entity.DiaryId,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.Description,
                entity.TotalCalories,
                entity.MealFoods.Select(mf => mf.ToDTO()).ToList());
        }

        public static Meal ToEntity(this CreateMealDTO dto)
        {
            Meal meal = new(dto.DiaryId, dto.Name, dto.Description);
            return meal;
        }
    }
}
