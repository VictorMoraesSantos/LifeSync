namespace Financial.Application.DTOs.Category
{
    public record CreateCategoryDTO(
        int UserId,
        string Name,
        string? Description);
}
