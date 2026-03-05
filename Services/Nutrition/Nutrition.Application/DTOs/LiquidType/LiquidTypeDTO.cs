using Core.Application.DTO;

namespace Nutrition.Application.DTOs.LiquidType
{
    public record LiquidTypeDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
