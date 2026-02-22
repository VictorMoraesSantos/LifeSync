using Core.Domain.Filters;
using Gym.Domain.Entities;

namespace Gym.Domain.Filters.Specifications
{
    public class ExerciseSpecification : Specification<Exercise, int>
    {
        public ExerciseSpecification(ExerciseQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, e => e.Id == filter.Id!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), e => e.Name.Contains(filter.NameContains!));
            AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), e => e.Description.Contains(filter.DescriptionContains!));
            AddIf(!string.IsNullOrWhiteSpace(filter.MuscleGroupContains), e => e.MuscleGroup.ToString().Contains(filter.MuscleGroupContains!));
            AddIf(!string.IsNullOrWhiteSpace(filter.ExerciseTypeContains), e => e.Type.ToString().Contains(filter.ExerciseTypeContains!));
            AddIf(!string.IsNullOrWhiteSpace(filter.EquipamentTypeContains), e => e.EquipmentType.ToString()!.Contains(filter.EquipamentTypeContains!));
        }
    }
}
