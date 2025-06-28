using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.Enums;
using Gym.Domain.Events;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class WorkoutSession : BaseEntity<int>, IAggregateRoot
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
        public WorkoutStatus Status { get; private set; }
        public int PlannedDurationMinutes { get; private set; }
        public int? ActualDurationMinutes { get; private set; }
        public string Notes { get; private set; }

        private readonly List<WorkoutActivity> _activities;
        public IReadOnlyCollection<WorkoutActivity> Activities => _activities.AsReadOnly();

        protected WorkoutSession()
        {
            _activities = new List<WorkoutActivity>();
        }

        public WorkoutSession(int userId, string name, string description, DateTime scheduledDate, int plannedDurationMinutes)
        {
            ValidateName(name);
            ValidateDescription(description);
            ValidateScheduledDate(scheduledDate);

            UserId = userId;
            Name = name;
            Description = description ?? "";
            ScheduledDate = scheduledDate;
            Status = WorkoutStatus.Planned;
            PlannedDurationMinutes = plannedDurationMinutes;
            Notes = "";

            _activities = new List<WorkoutActivity>();

            AddDomainEvent(new WorkoutCreatedEvent(Id, userId));
        }

        // Validações
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Workout name cannot be empty");

            if (name.Length > 100)
                throw new DomainException("Workout name cannot exceed 100 characters");
        }

        private void ValidateDescription(string description)
        {
            if (description?.Length > 500)
                throw new DomainException("Workout description cannot exceed 500 characters");
        }

        private void ValidateScheduledDate(DateTime scheduledDate)
        {
            if (scheduledDate.Date < DateTime.Today)
                throw new DomainException("Scheduled date cannot be in the past");
        }

        // Métodos de negócio
        public void UpdateBasicInfo(string name, string description)
        {
            ValidateName(name);
            ValidateDescription(description);

            Name = name;
            Description = description;
        }

        public void Reschedule(DateTime newScheduledDate)
        {
            ValidateScheduledDate(newScheduledDate);

            ScheduledDate = newScheduledDate;
        }

        public void StartWorkout()
        {
            if (Status != WorkoutStatus.Planned)
                throw new DomainException("Only planned workouts can be started");

            Status = WorkoutStatus.InProgress;
        }

        public void CompleteWorkout(int actualDurationMinutes, string notes = null)
        {
            if (Status == WorkoutStatus.Cancelled)
                throw new DomainException("Cancelled workouts cannot be completed");

            if (_activities.Count == 0)
                throw new DomainException("Cannot complete a workout with no activities");

            if (actualDurationMinutes <= 0)
                throw new DomainException("Actual duration must be greater than zero");

            Status = WorkoutStatus.Completed;
            CompletedDate = DateTime.UtcNow;
            ActualDurationMinutes = actualDurationMinutes;

            if (notes != null)
                Notes = notes;

            AddDomainEvent(new WorkoutCompletedEvent(Id, UserId, actualDurationMinutes));
        }

        public void CancelWorkout(string reason)
        {
            if (Status == WorkoutStatus.Completed)
                throw new DomainException("Completed workouts cannot be cancelled");

            Status = WorkoutStatus.Cancelled;
            Notes = reason ?? "No reason provided";

            AddDomainEvent(new WorkoutCancelledEvent(Id, reason));
        }

        public void AddActivity(
            int activityId,
            string activityName,
            int order,
            StrengthParameters strengthParameters = null,
            CardioParameters cardioParameters = null)
        {
            if (activityId <= 0)
                throw new DomainException("Activity ID cannot be cannot be negative");

            if (string.IsNullOrWhiteSpace(activityName))
                throw new DomainException("Activity name cannot be empty");

            if (strengthParameters == null && cardioParameters == null)
                throw new DomainException("Either strength or cardio parameters must be provided");

            var activity = new WorkoutActivity(Id, activityId, activityName, order, strengthParameters, cardioParameters);

            if (_activities.Any(a => a.Order == order))
            {
                // Reorder existing activities
                foreach (var existingActivity in _activities.Where(a => a.Order >= order))
                {
                    existingActivity.UpdateOrder(existingActivity.Order + 1);
                }
            }

            _activities.Add(activity);
        }

        public void RemoveActivity(int activityId)
        {
            var activity = _activities.FirstOrDefault(a => a.ActivityId == activityId);
            if (activity != null)
            {
                int removedOrder = activity.Order;
                _activities.Remove(activity);

                // Reorder remaining activities
                foreach (var remainingActivity in _activities.Where(a => a.Order > removedOrder))
                {
                    remainingActivity.UpdateOrder(remainingActivity.Order - 1);
                }
            }
        }

        public void ReorderActivity(int activityId, int newOrder)
        {
            if (newOrder <= 0)
                throw new DomainException("Order must be greater than zero");

            var activity = _activities.FirstOrDefault(a => a.ActivityId == activityId);
            if (activity == null)
                throw new DomainException("Activity not found in this workout");

            int oldOrder = activity.Order;

            if (oldOrder == newOrder)
                return;

            if (newOrder > oldOrder)
            {
                // Moving down the list
                foreach (var a in _activities.Where(x => x.Order > oldOrder && x.Order <= newOrder))
                {
                    a.UpdateOrder(a.Order - 1);
                }
            }
            else
            {
                // Moving up the list
                foreach (var a in _activities.Where(x => x.Order >= newOrder && x.Order < oldOrder))
                {
                    a.UpdateOrder(a.Order + 1);
                }
            }

            activity.UpdateOrder(newOrder);
        }

        public void CompleteActivity(int activityId, string notes = null)
        {
            var activity = _activities.FirstOrDefault(a => a.ActivityId == activityId);
            if (activity == null)
                throw new DomainException("Activity not found in this workout");

            activity.Complete(notes);
        }

        public void SkipActivity(int activityId, string reason = null)
        {
            var activity = _activities.FirstOrDefault(a => a.ActivityId == activityId);
            if (activity == null)
                throw new DomainException("Activity not found in this workout");

            activity.Skip(reason);
        }
    }
}
