using Nutrition.Application.DTOs.Meals;
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

        //public static Meal ToEntity(this CreateMealDTO dto)
        //{
        //    var meal = new Meal(dto.Name, dto.Description);
        //    // Se desejar, pode adicionar MealFoods depois
        //    return meal;
        //}
    }
}
