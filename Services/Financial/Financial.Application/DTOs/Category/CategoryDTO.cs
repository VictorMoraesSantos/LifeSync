using Core.Application.DTO;

namespace Financial.Application.DTOs.Category
{
    public record CategoryDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string? Description)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
