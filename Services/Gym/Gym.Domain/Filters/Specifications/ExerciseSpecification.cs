using Core.Domain.Filters;
using Gym.Domain.Entities;
using System.Linq.Expressions;

namespace Gym.Domain.Filters.Specifications
{
    public class ExerciseSpecification : BaseFilterSpecification<Exercise, int>
    {
        public ExerciseSpecification(ExerciseFilter filter)
            : base(filter, BuildCriteria)
        { }

        private static Expression<Func<Exercise, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (ExerciseFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<Exercise, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, e => e.Id == filter.Id!.Value)
                .AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), e => e.Name.Contains(filter.NameContains!))
                .AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), e => e.Description != null && e.Description.Contains(filter.DescriptionContains!))
                .AddIf(!string.IsNullOrWhiteSpace(filter.MuscleGroupContains), e => e.MuscleGroup.ToString().Contains(filter.MuscleGroupContains!))
                .AddIf(!string.IsNullOrWhiteSpace(filter.TypeContains), e => e.Type.ToString().Contains(filter.TypeContains!))
                .AddIf(!string.IsNullOrWhiteSpace(filter.EquipamentTypeContains), e => e.EquipmentType.ToString()!.Contains(filter.EquipamentTypeContains!));

            return builder.Build();
        }
    }
}
