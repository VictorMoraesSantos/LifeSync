namespace LifeSyncApp.Client.Models.Financial.Category
{
    public record CreateCategoryCommand(int UserId, string Name, string? Description);
}