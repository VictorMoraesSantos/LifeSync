using Core.Domain.Filters;
using Financial.Domain.Entities;
using System.Linq.Expressions;

namespace Financial.Domain.Filters.Specifications
{
    public class CategorySpecification : BaseFilterSpecification<Category, int>
    {
        public CategorySpecification(CategoryQueryFilter filter)
        : base(filter, BuildCriteria)
        { }

        private static Expression<Func<Category, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (CategoryQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<Category, int>(filter)
            .AddCommonFilters()
            .AddIf(filter.Id.HasValue, c => c.Id == filter.Id!.Value)
            .AddIf(filter.UserId.HasValue, c => c.UserId == filter.UserId!.Value)
            .AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), c => c.Name.Contains(filter.NameContains!))
            .AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), c => c.Description != null && c.Description.Contains(filter.DescriptionContains!));

            return builder.Build();
        }
    }
}
