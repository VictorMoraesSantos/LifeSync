namespace Financial.Application.DTOs.Category
{
    public record UpdateCategoryDTO(
        int Id,
        string Name,
        string? Description);
}
