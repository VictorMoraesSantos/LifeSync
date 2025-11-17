namespace LifeSyncApp.Client.Models.TaskManager.TaskItem
{
    public class UpdateTaskItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; } = Status.Pending;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        
        public UpdateTaskItemDTO() { }

        public UpdateTaskItemDTO(int id, string title, string description, Status status, Priority priority, DateOnly dueDate)
        {
            Id = id;
            Title = title;
            Description = description;
            Status = status;
            Priority = priority;
            DueDate = dueDate;
        }
    }
}

