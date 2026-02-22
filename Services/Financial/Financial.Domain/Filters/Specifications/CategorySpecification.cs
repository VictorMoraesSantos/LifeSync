using Core.Domain.Filters;
using Financial.Domain.Entities;

namespace Financial.Domain.Filters.Specifications
{
    public class CategorySpecification : Specification<Category, int>
    {
        public CategorySpecification(CategoryQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, c => c.Id == filter.Id!.Value);
            AddIf(filter.UserId.HasValue, c => c.UserId == filter.UserId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), c => c.Name.Contains(filter.NameContains!));
            AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), c => c.Description != null && c.Description.Contains(filter.DescriptionContains!));
        }
    }
}
