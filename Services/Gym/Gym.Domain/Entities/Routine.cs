using Core.Domain.Entities;
using Core.Domain.Exceptions;

namespace Gym.Domain.Entities
{
    public class Routine : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        private readonly List<RoutineExercise> _routineExercises = new();
        public IReadOnlyCollection<RoutineExercise> RoutineExercises => _routineExercises.AsReadOnly();

        private Routine() { }

        public Routine(
            int userId,
            string name,
            string description)
        {
            Validate(name);
            Validate(description);

            UserId = userId;
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