using TaskManager.Domain.Enums;

namespace TaskManager.Domain.ValueObjects
{
    public class TaskLabelFilter
    {
        public int? UserId { get; }
        public int? TaskItemId { get; }
        public string? NameContains { get; }
        public LabelColor? LabelColor { get; }

        public TaskLabelFilter(
            int? userId = null,
            int? taskItemId = null,
            string? nameContains = null,
            LabelColor? labelColor = null)
        {
            UserId = userId;
            TaskItemId = taskItemId;
            NameContains = nameContains;
            LabelColor = labelColor;
        }
    }
}
