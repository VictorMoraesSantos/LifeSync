namespace LifeSyncApp.Client.Models.TaskManager
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Priority { get; set; }
        public DateOnly DueDate { get; set; }
        public int UserId { get; set; }
        public List<TaskLabelDto> Labels { get; set; } = new();
    }

    public class CreateTaskItemRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateOnly DueDate { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateTaskItemRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Priority { get; set; }
        public DateOnly DueDate { get; set; }
    }

    public class TaskLabelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LabelColor { get; set; } = string.Empty;
        public int UserId { get; set; }
    }

    public class CreateTaskLabelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string LabelColor { get; set; } = "#007bff";
        public int UserId { get; set; }
    }

    public class UpdateTaskLabelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string LabelColor { get; set; } = "#007bff";
    }

    public enum TaskStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4
    }
}
