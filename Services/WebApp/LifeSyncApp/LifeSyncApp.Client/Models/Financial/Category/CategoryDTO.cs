namespace LifeSyncApp.Client.Models.Financial.Category
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}