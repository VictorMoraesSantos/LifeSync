namespace LifeSyncApp.DTOs.Financial.Category
{
    public record CategoryDTO(
        int Id,
        int UserId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string? Description);
}
