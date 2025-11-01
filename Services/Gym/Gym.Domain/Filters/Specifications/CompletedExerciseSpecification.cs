﻿using Core.Domain.Filters;
using Gym.Domain.Entities;
using System.Linq.Expressions;

namespace Gym.Domain.Filters.Specifications
{
    public class CompletedExerciseSpecification : BaseFilterSpecification<CompletedExercise, int>
    {
        public CompletedExerciseSpecification(CompletedExerciseFilter filter)
            : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<CompletedExercise, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (CompletedExerciseFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<CompletedExercise, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.TrainingSessionId.HasValue, ce => ce.TrainingSessionId == filter.TrainingSessionId!.Value)
                .AddIf(filter.RoutineExerciseId.HasValue, ce => ce.RoutineExerciseId == filter.RoutineExerciseId!.Value)
                .AddIf(filter.SetsCompletedEquals.HasValue, ce => ce.SetsCompleted.Value == filter.SetsCompletedEquals!.Value)
                .AddIf(filter.SetsCompletedLessThan.HasValue, ce => ce.SetsCompleted.Value < filter.SetsCompletedLessThan!.Value)
                .AddIf(filter.SetsCompletedGreaterThan.HasValue, ce => ce.SetsCompleted.Value > filter.SetsCompletedGreaterThan!.Value)
                .AddIf(filter.RepetitionsCompletedEquals.HasValue, ce => ce.RepetitionsCompleted.Value == filter.RepetitionsCompletedEquals!.Value)
                .AddIf(filter.RepetitionsCompletedLessThan.HasValue, ce => ce.RepetitionsCompleted.Value < filter.RepetitionsCompletedLessThan!.Value)
                .AddIf(filter.RepetitionsCompletedGreaterThan.HasValue, ce => ce.RepetitionsCompleted.Value > filter.RepetitionsCompletedGreaterThan!.Value)
                .AddIf(filter.WeightUsedCompletedEquals.HasValue, ce => ce.WeightUsed != null && ce.WeightUsed.Value == filter.WeightUsedCompletedEquals!.Value)
                .AddIf(filter.WeightUsedCompletedLessThan.HasValue, ce => ce.WeightUsed != null && ce.WeightUsed.Value < filter.WeightUsedCompletedLessThan!.Value)
                .AddIf(filter.WeightUsedCompletedGreaterThan.HasValue, ce => ce.WeightUsed != null && ce.WeightUsed.Value > filter.WeightUsedCompletedGreaterThan!.Value)
                .AddIf(filter.CompletedAt != default, ce => ce.CompletedAt.Date == filter.CompletedAt.Date)
                .AddIf(!string.IsNullOrWhiteSpace(filter.NotesContains), ce => ce.Notes != null && ce.Notes.Contains(filter.NotesContains!));

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<CompletedExercise, int> spec)
        {
            spec.AddInclude(ce => ce.TrainingSession);
            spec.AddInclude(ce => ce.RoutineExercise);
        }
    }
}
