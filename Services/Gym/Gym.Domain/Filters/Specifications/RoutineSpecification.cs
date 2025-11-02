using Core.Domain.Filters;
using Gym.Domain.Entities;
using System.Linq.Expressions;

namespace Gym.Domain.Filters.Specifications
{
    public class RoutineSpecification : BaseFilterSpecification<Routine, int>
    {
        public RoutineSpecification(RoutineQueryFilter filter)
            : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<Routine, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (RoutineQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<Routine, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, r => r.Id == filter.Id!.Value)
                .AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), r => r.Name.Contains(filter.NameContains))
                .AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), r => r.Description.Contains(filter.DescriptionContains))
                .AddIf(filter.RoutineExerciseId.HasValue, r => r.RoutineExercises.Any(re => re.Id == filter.RoutineExerciseId!.Value));

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<Routine, int> spec)
        {
            spec.AddInclude(r => r.RoutineExercises);
        }
    }
}
