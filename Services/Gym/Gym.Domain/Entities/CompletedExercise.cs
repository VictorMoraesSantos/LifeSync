using Core.Domain.Entities;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class CompletedExercise : BaseEntity<int>
    {
        public int TrainingSessionId { get; private set; }
        public TrainingSession TrainingSession { get; private set; }
        public int RoutineExerciseId { get; private set; }
        public RoutineExercise RoutineExercise { get; private set; }
        public SetCount SetsCompleted { get; private set; }
        public RepetitionCount RepetitionsCompleted { get; private set; }
        public Weight? WeightUsed { get; private set; }
        public DateTime CompletedAt { get; private set; } = DateTime.UtcNow;
        public string? Notes { get; private set; }

        protected CompletedExercise() { }

        public CompletedExercise(
            int trainingSessionId,
            int routineExerciseId,
            SetCount setsCompleted,
            RepetitionCount repetitionsCompleted,
            Weight? weightUsed = null,
            string? notes = null)
        {
            TrainingSessionId = trainingSessionId;
            RoutineExerciseId = routineExerciseId;
            SetsCompleted = setsCompleted;
            RepetitionsCompleted = repetitionsCompleted;
            WeightUsed = weightUsed;
            Notes = notes;
        }

        public void Update(
            SetCount setsCompleted,
            RepetitionCount repetitionsCompleted,
            Weight? weightUsed,
            string? notes = null)
        {
            SetsCompleted = setsCompleted;
            RepetitionsCompleted = repetitionsCompleted;
            WeightUsed = weightUsed;
            Notes = notes;
            MarkAsUpdated();
        }
    }
}