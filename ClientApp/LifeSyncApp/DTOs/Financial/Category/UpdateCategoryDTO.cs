namespace LifeSyncApp.DTOs.Financial.Category
{
    public record UpdateCategoryDTO(
        int Id,
        string Name,
        string? Description);
}
