using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class Workout : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? ScheduledDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
        private readonly List<WorkoutExercise> _exercises;
        public IReadOnlyCollection<WorkoutExercise> Exercises => _exercises.AsReadOnly();

        protected Workout()
        {
            _exercises = new List<WorkoutExercise>();
            CreatedDate = DateTime.UtcNow;
        }

        public Workout(string name, string description = null, DateTime? scheduledDate = null) : this()
        {
            UpdateName(name);
            Description = description ?? "";
            ScheduledDate = scheduledDate;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Workout name cannot be empty");

            Name = name;
        }

        public void UpdateDescription(string description)
        {
            Description = description ?? "";
        }

        public void ScheduleFor(DateTime date)
        {
            ScheduledDate = date;
        }

        public void Complete()
        {
            if (!_exercises.Any())
                throw new DomainException("Cannot complete a workout with no exercises");

            CompletedDate = DateTime.UtcNow;
        }

        public void AddExercise(Exercise exercise, SetRepetition setRepetition, int order)
        {
            if (exercise == null)
                throw new DomainException("Exercise cannot be null");

            var workoutExercise = new WorkoutExercise(Id, exercise.Id, setRepetition, order);
            _exercises.Add(workoutExercise);
        }

        public void RemoveExercise(int exerciseId)
        {
            var exercise = _exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);
            if (exercise != null)
                _exercises.Remove(exercise);
        }

        public void ReorderExercise(int exerciseId, int newOrder)
        {
            var exercise = _exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);
            if (exercise == null)
                throw new DomainException("Exercise not found in workout");

            exercise.UpdateOrder(newOrder);
        }
    }
}
