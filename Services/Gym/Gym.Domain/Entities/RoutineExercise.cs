using Core.Domain.Entities;
using Gym.Domain.ValueObjects;

namespace Gym.Domain.Entities
{
    public class RoutineExercise : BaseEntity<int>
    {
        public int RoutineId { get; private set; }
        public Routine Routine { get; private set; }

        public int ExerciseId { get; private set; }
        public Exercise Exercise { get; private set; }

        public SetCount Sets { get; private set; }
        public RepetitionCount Repetitions { get; private set; }
        public RestTime RestBetweenSets { get; private set; }
        public Weight? RecommendedWeight { get; private set; }
        public int Order { get; private set; }
        public string? Instructions { get; private set; }

        protected RoutineExercise() { }

        public RoutineExercise(
            int routineId,
            int exerciseId,
            SetCount sets,
            RepetitionCount repetitions,
            RestTime restBetweenSets,
            int order,
            Weight? recommendedWeight = null,
            string? instructions = null)
        {
            RoutineId = routineId;
            ExerciseId = exerciseId;
            Sets = sets;
            Repetitions = repetitions;
            RestBetweenSets = restBetweenSets;
            Order = order;
            RecommendedWeight = recommendedWeight;
            Instructions = instructions;
        }

        public void UpdateParameters(
            SetCount sets,
            RepetitionCount repetitions,
            RestTime restBetweenSets,
            Weight? recommendedWeight = null,
            string? instructions = null)
        {
            Sets = sets;
            Repetitions = repetitions;
            RestBetweenSets = restBetweenSets;
            RecommendedWeight = recommendedWeight;
            Instructions = instructions;
            MarkAsUpdated();
        }
    }
}