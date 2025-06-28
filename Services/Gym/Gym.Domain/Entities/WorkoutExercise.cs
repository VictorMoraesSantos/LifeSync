using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class WorkoutExercise : BaseEntity<int>
    {
        public int WorkoutId { get; private set; }
        public int ExerciseId { get; private set; }
        public SetRepetition SetRepetition { get; private set; }
        public int Order { get; private set; }
        public bool Completed { get; private set; }

        protected WorkoutExercise() { }

        public WorkoutExercise(int workoutId, int exerciseId, SetRepetition setRepetition, int order)
        {
            WorkoutId = workoutId;
            ExerciseId = exerciseId;
            SetRepetition = setRepetition;
            Order = order;
            Completed = false;
        }

        public void UpdateSetRepetition(SetRepetition setRepetition)
        {
            SetRepetition = setRepetition ?? throw new DomainException("Set repetition cannot be null");
        }

        public void UpdateOrder(int order)
        {
            if (order < 1)
                throw new DomainException("Order must be greater than zero");

            Order = order;
        }

        public void MarkAsCompleted()
        {
            Completed = true;
        }
    }
}
