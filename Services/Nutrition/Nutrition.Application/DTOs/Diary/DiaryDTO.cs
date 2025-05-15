using Core.Application.DTO;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.DTOs.Diary
{
    public record DiaryDTO(
        int Id,
        int UserId,
        DateOnly Date,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int TotalCalories,
        IList<MealDTO> Meals,
        IList<LiquidDTO> Liquids
        ) : DTOBase(Id, CreatedAt, UpdatedAt);
}
