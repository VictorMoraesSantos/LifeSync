using Core.Application.DTO;
using Nutrition.Application.DTOs.Liquids;
using Nutrition.Application.DTOs.Meals;

namespace Nutrition.Application.DTOs.Diaries
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
