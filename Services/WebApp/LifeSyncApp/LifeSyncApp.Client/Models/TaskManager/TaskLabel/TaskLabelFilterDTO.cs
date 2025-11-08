namespace LifeSyncApp.Client.Models.TaskManager.TaskLabel;


public class TaskLabelFilterDTO
{
    public int? Id { get; set; }
    public int? UserId { get; set; }
    public int? TaskItemId { get; set; }
    public string? NameContains { get; set; }
    public LabelColor? LabelColor { get; set; }
    public DateOnly? CreatedAt { get; set; }
    public DateOnly? UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
    public string? SortBy { get; set; }
    public bool? SortDesc { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
