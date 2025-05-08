namespace Core.Application.DTO
{
    public record DTOBase(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
