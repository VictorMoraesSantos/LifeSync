using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.Events;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class ActivityExecution : BaseEntity<int>, IAggregateRoot
    {
        public int UserId { get; private set; }
        public int ActivityId { get; private set; }
        public string ActivityName { get; private set; }
        public int? WorkoutSessionId { get; private set; }
        public DateTime ExecutionDate { get; private set; }
        public string Notes { get; private set; }
        public int PerceivedExertion { get; private set; } // Escala 0-10
        public int CaloriesBurned { get; private set; }

        // Dados específicos por tipo de execução
        public StrengthExecution StrengthData { get; private set; }
        public CardioExecution CardioData { get; private set; }

        protected ActivityExecution() { }

        // Construtor para execução de musculação
        public ActivityExecution(
            int userId,
            int activityId,
            string activityName,
            StrengthExecution strengthData,
            DateTime executionDate,
            int? workoutSessionId = null,
            string notes = null)
        {
            ValidateBasicInfo(userId, activityId, activityName, executionDate);

            UserId = userId;
            ActivityId = activityId;
            ActivityName = activityName;
            WorkoutSessionId = workoutSessionId;
            ExecutionDate = executionDate;
            Notes = notes ?? "";
            PerceivedExertion = 0;
            CaloriesBurned = 0;
            StrengthData = strengthData ?? throw new DomainException("Strength data cannot be null");
            CardioData = null;

            AddDomainEvent(new ActivityExecutionRecordedEvent(Id, userId, activityId));
        }

        // Construtor para execução de corrida
        public ActivityExecution(
            int userId,
            int activityId,
            string activityName,
            CardioExecution cardioData,
            DateTime executionDate,
            int? workoutSessionId = null,
            string notes = null)
        {
            ValidateBasicInfo(userId, activityId, activityName, executionDate);

            UserId = userId;
            ActivityId = activityId;
            ActivityName = activityName;
            WorkoutSessionId = workoutSessionId;
            ExecutionDate = executionDate;
            Notes = notes ?? "";
            PerceivedExertion = 0;
            CaloriesBurned = 0;
            StrengthData = null;
            CardioData = cardioData ?? throw new DomainException("Cardio data cannot be null");

            AddDomainEvent(new ActivityExecutionRecordedEvent(Id, userId, activityId));
        }

        // Validações
        private void ValidateBasicInfo(int userId, int activityId, string activityName, DateTime executionDate)
        {
            if (userId <= 0)
                throw new DomainException("User ID cannot be negatuve");

            if (activityId <= 0)
                throw new DomainException("Activity ID cannot be negative");

            if (string.IsNullOrWhiteSpace(activityName))
                throw new DomainException("Activity name cannot be empty");

            if (executionDate > DateTime.UtcNow)
                throw new DomainException("Execution date cannot be in the future");
        }

        private void ValidatePerceivedExertion(int value)
        {
            if (value < 0 || value > 10)
                throw new DomainException("Perceived exertion must be between 0 and 10");
        }

        // Métodos de negócio
        public void UpdateNotes(string notes)
        {
            Notes = notes ?? "";
        }

        public void SetPerceivedExertion(int value)
        {
            ValidatePerceivedExertion(value);
            PerceivedExertion = value;
        }

        public void SetCaloriesBurned(int calories)
        {
            if (calories < 0)
                throw new DomainException("Calories burned cannot be negative");

            CaloriesBurned = calories;
        }

        // Métodos específicos para tipos de execução
        public void UpdateStrengthData(StrengthExecution strengthData)
        {
            if (CardioData != null)
                throw new DomainException("Cannot update strength data for a cardio execution");

            StrengthData = strengthData ?? throw new DomainException("Strength data cannot be null");
        }

        public void UpdateCardioData(CardioExecution cardioData)
        {
            if (StrengthData != null)
                throw new DomainException("Cannot update cardio data for a strength execution");

            CardioData = cardioData ?? throw new DomainException("Cardio data cannot be null");
        }
    }
}
