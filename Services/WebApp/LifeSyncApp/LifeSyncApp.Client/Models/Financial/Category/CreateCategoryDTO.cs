namespace LifeSyncApp.Client.Models.Financial.Category
{
    public class CreateCategoryDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}