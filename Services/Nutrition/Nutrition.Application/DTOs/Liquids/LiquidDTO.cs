using Core.Application.DTO;

namespace Nutrition.Application.DTOs.Liquids
{
    public record LiquidDTO(
        int Id,
        int DiaryId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        int QuantityMl,
        int CaloriesPerMl,
        int TotalCalories
        ) : DTOBase(Id, CreatedAt, UpdatedAt);
}
