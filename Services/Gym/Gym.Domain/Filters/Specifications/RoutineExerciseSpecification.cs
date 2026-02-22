using Core.Domain.Filters;
using Gym.Domain.Entities;

namespace Gym.Domain.Filters.Specifications
{
    public class RoutineExerciseSpecification : Specification<RoutineExercise, int>
    {
        public RoutineExerciseSpecification(RoutineExerciseQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, re => re.Id == filter.Id!.Value);
            AddIf(filter.RoutineId.HasValue, re => re.RoutineId == filter.RoutineId!.Value);
            AddIf(filter.ExerciseId.HasValue, re => re.ExerciseId == filter.ExerciseId!.Value);
            AddIf(filter.SetsEquals.HasValue, re => re.Sets.Value <= filter.SetsEquals!.Value);
            AddIf(filter.SetsLessThan.HasValue, re => re.Sets.Value < filter.SetsLessThan!.Value);
            AddIf(filter.SetsGreaterThan.HasValue, re => re.Sets.Value > filter.SetsGreaterThan!.Value);
            AddIf(filter.RepetitionsEquals.HasValue, re => re.Repetitions.Value == filter.RepetitionsEquals!.Value);
            AddIf(filter.RepetitionsLessThan.HasValue, re => re.Repetitions.Value < filter.RepetitionsLessThan!.Value);
            AddIf(filter.RepetitionsGreaterThan.HasValue, re => re.Repetitions.Value > filter.RepetitionsGreaterThan!.Value);
            AddIf(filter.RestTimeEquals.HasValue, re => re.RestBetweenSets.Value == filter.RestTimeEquals!.Value);
            AddIf(filter.RestTimeLessThan.HasValue, re => re.RestBetweenSets.Value < filter.RestTimeLessThan!.Value);
            AddIf(filter.RestTimeGreaterThan.HasValue, re => re.RestBetweenSets.Value > filter.RestTimeGreaterThan!.Value);
            AddIf(filter.RecommendedWeightEquals.HasValue, re => re.RecommendedWeight.Value == filter.RecommendedWeightEquals!.Value);
            AddIf(filter.RecommendedWeightLessThan.HasValue, re => re.RecommendedWeight.Value < filter.RecommendedWeightLessThan!.Value);
            AddIf(filter.RecommendedWeightGreaterThan.HasValue, re => re.RecommendedWeight.Value > filter.RecommendedWeightGreaterThan!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.InstructionsContains), re => re.Instructions.Contains(filter.InstructionsContains!));
            AddInclude(re => re.Routine);
            AddInclude(re => re.Exercise);
        }
    }
}
