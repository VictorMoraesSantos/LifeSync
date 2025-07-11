using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class Routine : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int UserId { get; private set; }

        private readonly List<RoutineExercise> _routineExercises = new();
        public IReadOnlyCollection<RoutineExercise> RoutineExercises => _routineExercises.AsReadOnly();

        private Routine() { }

        public Routine(
            string name,
            string description,
            int userId)
        {
            Validate(name);
            Validate(description);

            Name = name;
            Description = description;
            UserId = userId;
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

        public void AddExercise(
            int exerciseId,
            SetCount sets,
            RepetitionCount repetitions,
            RestTime restBetweenSets,
            Weight? recommendedWeight = null,
            string? instructions = null)
        {
            var routineExercise = new RoutineExercise(
                Id,
                exerciseId,
                sets,
                repetitions,
                restBetweenSets,
                recommendedWeight,
                instructions);

            _routineExercises.Add(routineExercise);
            MarkAsUpdated();
        }

        public void RemoveExercise(int routineExerciseId)
        {
            var routineExercise = _routineExercises.FirstOrDefault(re => re.Id == routineExerciseId);
            if (routineExercise == null)
                throw new DomainException("Routine exercise cannot be found");

            _routineExercises.Remove(routineExercise);
            MarkAsUpdated();
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{value} cannot be null");

            return;
        }
    }
}