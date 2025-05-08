using Core.Application.DTO;
using Nutrition.Application.DTOs.MealFoods;

namespace Nutrition.Application.DTOs.Meals
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
