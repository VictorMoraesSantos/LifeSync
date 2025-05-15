using Core.Application.DTO;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.DTOs.Meal
{
    public record MealDTO(
        int Id,
        int DiaryId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string Description,
        int TotalCalories,
        IList<MealFoodDTO> MealFoods
        ) : DTOBase(Id, CreatedAt, UpdatedAt);
}
