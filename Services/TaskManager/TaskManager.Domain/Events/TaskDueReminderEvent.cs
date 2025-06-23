using BuildingBlocks.Messaging.Abstractions;

namespace TaskManager.Domain.Events
{
    public class TaskDueReminderEvent : IntegrationEvent
    {
        public int TaskId { get; }
        public int UserId { get; }
        public string TaskTitle { get; }
        public DateOnly DueDate { get; }

        public TaskDueReminderEvent(int taskId, int userId, string taskTitle, DateOnly dueDate)
        {
            TaskId = taskId;
            UserId = userId;
            TaskTitle = taskTitle;
            DueDate = dueDate;
        }
    }
}
