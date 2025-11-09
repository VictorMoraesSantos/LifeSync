namespace LifeSyncApp.Client.Models.Financial.Category
{
    public record UpdateCategoryCommand(int Id, string Name, string? Description);
}