using Core.Domain.Filters;
using Nutrition.Domain.Entities;

namespace Nutrition.Domain.Filters.Specifications
{
    public class LiquidTypeSpecification : Specification<LiquidType, int>
    {
        public LiquidTypeSpecification(LiquidTypeQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, lt => lt.Id == filter.Id!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), lt => lt.Name.Contains(filter.NameContains!));
        }
    }
}
