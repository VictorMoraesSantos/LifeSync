namespace LifeSyncApp.Client.Models.TaskManager.TaskItem
{
    public class TaskItemFilterDTO
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? TitleContains { get; set; }
        public Status? Status { get; set; }
        public Priority? Priority { get; set; }
        public DateOnly? DueDate { get; set; }
        public int? LabelId { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
