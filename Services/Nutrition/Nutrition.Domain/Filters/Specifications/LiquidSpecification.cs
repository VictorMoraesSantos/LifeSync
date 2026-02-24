using Core.Domain.Filters;
using Nutrition.Domain.Entities;

namespace Nutrition.Domain.Filters.Specifications
{
    public class LiquidSpecification : Specification<Liquid, int>
    {
        public LiquidSpecification(LiquidQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, l => l.Id == filter.Id!.Value);
            AddIf(filter.DiaryId.HasValue, l => l.DiaryId == filter.DiaryId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), l => l.LiquidType.Name.Contains(filter.NameContains!));
            AddIf(filter.QuantityEquals.HasValue, l => l.Quantity == filter.QuantityEquals!.Value);
            AddIf(filter.QuantityGreaterThan.HasValue, l => l.Quantity > filter.QuantityGreaterThan!.Value);
            AddIf(filter.QuantityLessThan.HasValue, l => l.Quantity < filter.QuantityLessThan!.Value);
        }
    }
}
