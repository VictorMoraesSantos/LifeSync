namespace LifeSyncApp.Client.Models.Financial.Category
{
    public record CategoryDTO(
        int Id,
        int UserId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        string? Description);
}