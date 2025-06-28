using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.Enums;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class WorkoutActivity : BaseEntity<int>
    {
        public int WorkoutId { get; private set; }
        public int ActivityId { get; private set; }
        public string ActivityName { get; private set; }
        public int Order { get; private set; }
        public ActivityStatus Status { get; private set; }
        public string Notes { get; private set; }

        // Parâmetros específicos para diferentes tipos de atividades
        public StrengthParameters StrengthParameters { get; private set; }
        public CardioParameters CardioParameters { get; private set; }

        protected WorkoutActivity() { }

        public WorkoutActivity(
            int workoutId,
            int activityId,
            string activityName,
            int order,
            StrengthParameters strengthParameters = null,
            CardioParameters cardioParameters = null)
        {
            if (workoutId <= 0)
                throw new DomainException("Workout ID cannot be negative");

            if (activityId <= 0)
                throw new DomainException("Activity ID cannot be negative");

            if (string.IsNullOrWhiteSpace(activityName))
                throw new DomainException("Activity name cannot be empty");

            if (order <= 0)
                throw new DomainException("Order must be greater than zero");

            if (strengthParameters == null && cardioParameters == null)
                throw new DomainException("Either strength or cardio parameters must be provided");

            WorkoutId = workoutId;
            ActivityId = activityId;
            ActivityName = activityName;
            Order = order;
            Status = ActivityStatus.Planned;
            Notes = "";
            StrengthParameters = strengthParameters;
            CardioParameters = cardioParameters;
        }

        public void UpdateOrder(int newOrder)
        {
            if (newOrder <= 0)
                throw new DomainException("Order must be greater than zero");

            Order = newOrder;
        }

        public void Complete(string notes = null)
        {
            if (Status == ActivityStatus.Skipped)
                throw new DomainException("Skipped activities cannot be completed");

            Status = ActivityStatus.Completed;

            if (notes != null)
                Notes = notes;
        }

        public void Skip(string reason = null)
        {
            if (Status == ActivityStatus.Completed)
                throw new DomainException("Completed activities cannot be skipped");

            Status = ActivityStatus.Skipped;
            Notes = reason ?? "No reason provided";
        }

        public void UpdateStrengthParameters(StrengthParameters parameters)
        {
            if (CardioParameters != null)
                throw new DomainException("Cannot set strength parameters for a cardio activity");

            StrengthParameters = parameters ?? throw new DomainException("Strength parameters cannot be null");
        }

        public void UpdateCardioParameters(CardioParameters parameters)
        {
            if (StrengthParameters != null)
                throw new DomainException("Cannot set cardio parameters for a strength activity");

            CardioParameters = parameters ?? throw new DomainException("Cardio parameters cannot be null");
        }
    }
}
