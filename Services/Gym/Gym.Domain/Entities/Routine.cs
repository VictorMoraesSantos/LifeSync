using Core.Domain.Entities;
using Core.Domain.Exceptions;

namespace Gym.Domain.Entities
{
    public class Routine : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        private readonly List<RoutineExercise> _routineExercises = new();
        public IReadOnlyCollection<RoutineExercise> RoutineExercises => _routineExercises.AsReadOnly();

        protected Routine() { }

        public Routine(
            string name,
            string description)
        {
            Validate(name);
            Validate(description);

            Name = name;
            Description = description;
        }

        public void Update(
            string name,
            string description)
        {
            Validate(name);
            Validate(description);

            Name = name;
            Description = description;
        }

        public void AddExercise(RoutineExercise routineExercise)
        {
            if (routineExercise == null)
                throw new DomainException("Routine exercise cannot be null");

            _routineExercises.Add(routineExercise);
            MarkAsUpdated();
        }

        public void RemoveExercise(RoutineExercise routineExercise)
        {
            if (routineExercise == null)
                throw new DomainException("Routine exercise cannot be found");

            _routineExercises.Remove(routineExercise);
            MarkAsUpdated();
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
        }
    }
}