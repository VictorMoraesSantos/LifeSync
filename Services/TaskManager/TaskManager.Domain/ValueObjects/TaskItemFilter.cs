using TaskManager.Domain.Enums;

namespace TaskManager.Domain.ValueObjects
{
    public class TaskItemFilter
    {
        public int? UserId { get; }
        public string? TitleContains { get; }
        public Status? Status { get; }
        public Priority? Priority { get; }
        public DateOnly? DueDate { get; }
        public int? LabelId { get; }

        public TaskItemFilter(
            int? userId = null,
            string? titleContains = null,
            Status? status = null,
            Priority? priority = null,
            DateOnly? dueDate = null,
            int? labelId = null)
        {
            UserId = userId;
            TitleContains = titleContains;
            Status = status;
            Priority = priority;
            DueDate = dueDate;
            LabelId = labelId;
        }
    }
}