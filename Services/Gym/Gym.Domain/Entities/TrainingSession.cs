using Core.Domain.Entities;
using Core.Domain.Exceptions;

namespace Gym.Domain.Entities
{
    public class TrainingSession : BaseEntity<int>
    {
        public int UserId { get; private set; }

        public int RoutineId { get; private set; }
        public Routine Routine { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public string? Notes { get; private set; }

        private readonly List<CompletedExercise?> _completedExercises = new();
        public IReadOnlyCollection<CompletedExercise?> CompletedExercises => _completedExercises.AsReadOnly();

        private TrainingSession() { }

        public TrainingSession(
            int userId,
            int routineId,
            DateTime startTime,
            DateTime? endTime)
        {
            UserId = userId;
            RoutineId = routineId;
            ValidateDate(startTime, endTime);
            StartTime = startTime;
            EndTime = endTime;
        }

        public void Update(
            int routineId,
            DateTime startTime,
            DateTime? endTime,
            string notes)
        {
            RoutineId = routineId;
            ValidateDate(startTime, endTime);
            StartTime = startTime;
            EndTime = endTime;
            Notes = notes;
            MarkAsUpdated();
        }

        public void AddCompletedExercise(CompletedExercise completedExercise)
        {
            if (completedExercise == null)
                throw new DomainException("Completed exercise cannot be null");

            _completedExercises.Add(completedExercise);
            MarkAsUpdated();
        }

        public void Complete(string? notes = null)
        {
            if (EndTime.HasValue)
                return;

            EndTime = DateTime.UtcNow;
            Notes = notes;
            MarkAsUpdated();

            // Se estiver usando eventos de domínio
            // AddDomainEvent(new TrainingSessionCompletedEvent(UserId, Id));
        }

        public TimeSpan GetDuration()
        {
            return (EndTime ?? DateTime.UtcNow) - StartTime;
        }

        private void ValidateDate(DateTime startTime, DateTime? endTime)
        {
            if (endTime.HasValue && endTime <= startTime)
                throw new DomainException("End time must be after start time");
        }
    }
}