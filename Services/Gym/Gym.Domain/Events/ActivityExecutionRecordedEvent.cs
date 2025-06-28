using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class ActivityExecutionRecordedEvent : DomainEvent
    {
        public int ExecutionId { get; }
        public int UserId { get; }
        public int ActivityId { get; }

        public ActivityExecutionRecordedEvent(int executionId, int userId, int activityId)
        {
            ExecutionId = executionId;
            UserId = userId;
            ActivityId = activityId;
        }
    }
}
