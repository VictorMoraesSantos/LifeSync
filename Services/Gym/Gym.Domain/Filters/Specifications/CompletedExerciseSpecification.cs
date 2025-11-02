using Core.Domain.Filters;
using Gym.Domain.Entities;
using System.Linq.Expressions;

namespace Gym.Domain.Filters.Specifications
{
    public class CompletedExerciseSpecification : BaseFilterSpecification<CompletedExercise, int>
    {
        public CompletedExerciseSpecification(CompletedExerciseQueryFilter filter)
            : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<CompletedExercise, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (CompletedExerciseQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<CompletedExercise, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, ce => ce.Id == filter.Id!.Value)
                .AddIf(filter.TrainingSessionId.HasValue, ce => ce.TrainingSessionId == filter.TrainingSessionId!.Value)
                .AddIf(filter.RoutineExerciseId.HasValue, ce => ce.RoutineExerciseId == filter.RoutineExerciseId!.Value)
                .AddIf(filter.SetsCompletedEquals.HasValue, ce => ce.SetsCompleted.Value == filter.SetsCompletedEquals!.Value)
                .AddIf(filter.SetsCompletedLessThan.HasValue, ce => ce.SetsCompleted.Value < filter.SetsCompletedLessThan!.Value)
                .AddIf(filter.SetsCompletedGreaterThan.HasValue, ce => ce.SetsCompleted.Value > filter.SetsCompletedGreaterThan!.Value)
                .AddIf(filter.RepetitionsCompletedEquals.HasValue, ce => ce.RepetitionsCompleted.Value == filter.RepetitionsCompletedEquals!.Value)
                .AddIf(filter.RepetitionsCompletedLessThan.HasValue, ce => ce.RepetitionsCompleted.Value < filter.RepetitionsCompletedLessThan!.Value)
                .AddIf(filter.RepetitionsCompletedGreaterThan.HasValue, ce => ce.RepetitionsCompleted.Value > filter.RepetitionsCompletedGreaterThan!.Value)
                .AddIf(filter.WeightUsedCompletedEquals.HasValue, ce => ce.WeightUsed.Value == filter.WeightUsedCompletedEquals!.Value)
                .AddIf(filter.WeightUsedCompletedLessThan.HasValue, ce => ce.WeightUsed.Value < filter.WeightUsedCompletedLessThan!.Value)
                .AddIf(filter.WeightUsedCompletedGreaterThan.HasValue, ce => ce.WeightUsed.Value > filter.WeightUsedCompletedGreaterThan!.Value)
                .AddIf(filter.CompletedAt != default, ce => DateOnly.FromDateTime(ce.CompletedAt.Date) == filter.CompletedAt)
                .AddIf(!string.IsNullOrWhiteSpace(filter.NotesContains), ce => ce.Notes.Contains(filter.NotesContains!));

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<CompletedExercise, int> spec)
        {
            spec.AddInclude(ce => ce.TrainingSession);
            spec.AddInclude(ce => ce.RoutineExercise);
        }
    }
}
