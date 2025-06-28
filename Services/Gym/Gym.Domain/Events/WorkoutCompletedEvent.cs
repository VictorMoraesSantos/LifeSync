using Core.Domain.Events;

namespace Gym.Domain.Events
{
    public class WorkoutCompletedEvent : DomainEvent
    {
        public int WorkoutId { get; }
        public int UserId { get; }
        public int DurationMinutes { get; }

        public WorkoutCompletedEvent(int workoutId, int userId, int durationMinutes)
        {
            WorkoutId = workoutId;
            UserId = userId;
            DurationMinutes = durationMinutes;
        }
    }
}
