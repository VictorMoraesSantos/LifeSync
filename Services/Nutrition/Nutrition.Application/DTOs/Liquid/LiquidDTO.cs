using Core.Application.DTO;

namespace Nutrition.Application.DTOs.Liquid
{
    public record LiquidDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int DiaryId,
        string Name,
        int Quantity)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
