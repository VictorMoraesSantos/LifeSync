namespace LifeSyncApp.DTOs.Financial
{
    public class CreateCategoryDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
