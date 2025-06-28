using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class ProgressRecordedEvent : DomainEvent
    {
        public int ProgressId { get; }
        public int UserId { get; }

        public ProgressRecordedEvent(int progressId, int userId)
        {
            ProgressId = progressId;
            UserId = userId;
        }
    }
}
