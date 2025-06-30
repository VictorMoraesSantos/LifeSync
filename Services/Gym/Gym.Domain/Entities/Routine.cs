using Core.Domain.Entities;
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
            Name = name;
            Description = description;
            UserId = userId;
        }

        public void AddExercise(
            int exerciseId,
            SetCount sets,
            RepetitionCount repetitions,
            RestTime restBetweenSets,
            int order,
            Weight? recommendedWeight = null,
            string? instructions = null)
        {
            var routineExercise = new RoutineExercise(
                Id,
                exerciseId,
                sets,
                repetitions,
                restBetweenSets,
                order,
                recommendedWeight,
                instructions);

            _routineExercises.Add(routineExercise);
            MarkAsUpdated();
        }

        public void RemoveExercise(int routineExerciseId)
        {
            var routineExercise = _routineExercises.FirstOrDefault(re => re.Id == routineExerciseId);
            if (routineExercise != null)
            {
                _routineExercises.Remove(routineExercise);
                MarkAsUpdated();
            }
        }

        public int GetNextExerciseOrder()
        {
            return _routineExercises.Any() ? _routineExercises.Max(e => e.Order) + 1 : 1;
        }
    }
}