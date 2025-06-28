using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class ExerciseExecution : BaseEntity<int>
    {
        public int ExerciseId { get; private set; }
        public int? WorkoutId { get; private set; }
        public SetRepetition SetRepetition { get; private set; }
        public DateTime ExecutionDate { get; private set; }
        public string Notes { get; private set; }
        public bool Completed { get; private set; }
        public int? PerceivedExertion { get; private set; } // Scale 1-10

        protected ExerciseExecution() { }

        public ExerciseExecution(
            int exerciseId,
            SetRepetition setRepetition,
            DateTime executionDate,
            string notes = null,
            int? workoutId = null)
        {
            ExerciseId = exerciseId;
            SetRepetition = setRepetition;
            ExecutionDate = executionDate;
            Notes = notes ?? "";
            WorkoutId = workoutId;
            Completed = false;
        }

        public void Complete(int perceivedExertion = 0)
        {
            if (perceivedExertion < 0 || perceivedExertion > 10)
                throw new DomainException("Perceived exertion must be between 0 and 10");

            Completed = true;
            PerceivedExertion = perceivedExertion > 0 ? perceivedExertion : null;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes ?? "";
        }

        public void UpdateSetRepetition(SetRepetition setRepetition)
        {
            SetRepetition = setRepetition ?? throw new DomainException("Set repetition cannot be null");
        }
    }
}
