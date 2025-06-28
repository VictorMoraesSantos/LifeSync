using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class WorkoutCreatedEvent : DomainEvent
    {
        public int WorkoutId { get; }
        public int UserId { get; }

        public WorkoutCreatedEvent(int workoutId, int userId)
        {
            WorkoutId = workoutId;
            UserId = userId;
        }
    }
}
