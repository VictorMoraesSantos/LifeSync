using Core.Domain.Filters;
using Gym.Domain.Entities;
using System.Linq.Expressions;

namespace Gym.Domain.Filters.Specifications
{
    public class TrainingSessionSpecification : BaseFilterSpecification<TrainingSession, int>
    {
        public TrainingSessionSpecification(TrainingSessionQueryFilter filter)
        : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<TrainingSession, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (TrainingSessionQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<TrainingSession, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, r => r.Id == filter.Id.Value)
                .AddIf(filter.UserId.HasValue, r => r.UserId == filter.UserId.Value)
                .AddIf(filter.RoutineId.HasValue, r => r.RoutineId == filter.RoutineId.Value)
                .AddIf(filter.StartTime.HasValue, r => DateOnly.FromDateTime(r.StartTime) >= filter.StartTime.Value)
                .AddIf(filter.EndTime.HasValue, r => DateOnly.FromDateTime(r.EndTime.Value) <= filter.EndTime.Value)
                .AddIf(!string.IsNullOrWhiteSpace(filter.NotesContains), r => r.Notes.Contains(filter.NotesContains!));

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<TrainingSession, int> spec)
        {
            spec.AddInclude(r => r.CompletedExercises);
        }
    }
}
