using Core.Domain.Filters;
using Gym.Domain.Entities;
using System.Linq.Expressions;

namespace Gym.Domain.Filters.Specifications
{
    public class RoutineExerciseSpecification : BaseFilterSpecification<RoutineExercise, int>
    {
        public RoutineExerciseSpecification(RoutineExerciseQueryFilter filter)
            : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<RoutineExercise, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (RoutineExerciseQueryFilter)baseFilter;

            var builder = new FilterCriteriaBuilder<RoutineExercise, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, re => re.Id == filter.Id!.Value)
                .AddIf(filter.RoutineId.HasValue, re => re.RoutineId == filter.RoutineId!.Value)
                .AddIf(filter.ExerciseId.HasValue, re => re.ExerciseId == filter.ExerciseId!.Value)
                .AddIf(filter.SetsLessEquals.HasValue, re => re.Sets.Value <= filter.SetsLessEquals!.Value)
                .AddIf(filter.SetsLessThan.HasValue, re => re.Sets.Value < filter.SetsLessThan!.Value)
                .AddIf(filter.SetsGreaterThan.HasValue, re => re.Sets.Value > filter.SetsGreaterThan!.Value)
                .AddIf(filter.RepetitionsEquals.HasValue, re => re.Repetitions.Value == filter.RepetitionsEquals!.Value)
                .AddIf(filter.RepetitionsLessThan.HasValue, re => re.Repetitions.Value < filter.RepetitionsLessThan!.Value)
                .AddIf(filter.RepetitionsGreaterThan.HasValue, re => re.Repetitions.Value > filter.RepetitionsGreaterThan!.Value)
                .AddIf(filter.RestTimeEquals.HasValue, re => re.RestBetweenSets.Value == filter.RestTimeEquals!.Value)
                .AddIf(filter.RestTimeLessThan.HasValue, re => re.RestBetweenSets.Value < filter.RestTimeLessThan!.Value)
                .AddIf(filter.RestTimeGreaterThan.HasValue, re => re.RestBetweenSets.Value > filter.RestTimeGreaterThan!.Value)
                .AddIf(filter.RecommendedWeightEquals.HasValue, re => re.RecommendedWeight.Value == filter.RecommendedWeightEquals!.Value)
                .AddIf(filter.RecommendedWeightLessThan.HasValue, re => re.RecommendedWeight.Value < filter.RecommendedWeightLessThan!.Value)
                .AddIf(filter.RecommendedWeightGreaterThan.HasValue, re => re.RecommendedWeight.Value > filter.RecommendedWeightGreaterThan!.Value)
                .AddIf(!string.IsNullOrWhiteSpace(filter.InstructionsContains), re => re.Instructions.Contains(filter.InstructionsContains));

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<RoutineExercise, int> spec)
        {
            spec.AddInclude(re => re.Routine);
            spec.AddInclude(re => re.Exercise);
        }
    }
}
