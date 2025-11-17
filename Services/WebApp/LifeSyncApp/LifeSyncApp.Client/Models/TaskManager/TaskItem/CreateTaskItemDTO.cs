namespace LifeSyncApp.Client.Models.TaskManager.TaskItem
{
    public class CreateTaskItemDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public int UserId { get; set; }
        
        public CreateTaskItemDTO(string title, string description, Priority priority, DateOnly dueDate, int userId)
        {
            Title = title;
            Description = description;
            Priority = priority;
            DueDate = dueDate;
            UserId = userId;
        }
    }
}
