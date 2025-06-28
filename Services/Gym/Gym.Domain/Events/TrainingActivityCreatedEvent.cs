using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class TrainingActivityCreatedEvent : DomainEvent
    {
        public int ActivityId { get; }
        public string ActivityName { get; }

        public TrainingActivityCreatedEvent(int activityId, string activityName)
        {
            ActivityId = activityId;
            ActivityName = activityName;
        }
    }
}
