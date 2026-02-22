using Core.Domain.Filters;
using Gym.Domain.Entities;

namespace Gym.Domain.Filters.Specifications
{
    public class RoutineSpecification : Specification<Routine, int>
    {
        public RoutineSpecification(RoutineQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, r => r.Id == filter.Id!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), r => r.Name.Contains(filter.NameContains!));
            AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), r => r.Description.Contains(filter.DescriptionContains!));
            AddIf(filter.RoutineExerciseId.HasValue, r => r.RoutineExercises.Any(re => re.Id == filter.RoutineExerciseId!.Value));
            AddInclude(r => r.RoutineExercises);
        }
    }
}
