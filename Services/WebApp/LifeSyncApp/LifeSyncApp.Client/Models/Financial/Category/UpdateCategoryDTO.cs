namespace LifeSyncApp.Client.Models.Financial.Category
{
    public class UpdateCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}