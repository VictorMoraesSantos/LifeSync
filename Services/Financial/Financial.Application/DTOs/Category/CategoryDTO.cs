using Core.Application.DTO;

namespace Financial.Application.DTOs.Category
{
    public record CategoryDTO(
        int Id,
        int UserId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string? Description)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
