using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class TrainingActivityUpdatedEvent : DomainEvent
    {
        public int ActivityId { get; }

        public TrainingActivityUpdatedEvent(int activityId)
        {
            ActivityId = activityId;
        }
    }
}
