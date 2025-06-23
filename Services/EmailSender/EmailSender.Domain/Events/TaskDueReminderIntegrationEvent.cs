using BuildingBlocks.Messaging.Abstractions;

namespace EmailSender.Domain.Events
{
    public class TaskDueReminderIntegrationEvent : IntegrationEvent
    {
        public int TaskId { get; }
        public int UserId { get; }
        public string TaskTitle { get; }
        public DateOnly DueDate { get; }

        public TaskDueReminderIntegrationEvent(int taskId, int userId, string taskTitle, DateOnly dueDate)
        {
            TaskId = taskId;
            UserId = userId;
            TaskTitle = taskTitle;
            DueDate = dueDate;
        }
    }
}
