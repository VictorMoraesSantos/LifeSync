using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class WorkoutCancelledEvent : DomainEvent
    {
        public int WorkoutId { get; }
        public string Reason { get; }

        public WorkoutCancelledEvent(int workoutId, string reason)
        {
            WorkoutId = workoutId;
            Reason = reason;
        }
    }
}
