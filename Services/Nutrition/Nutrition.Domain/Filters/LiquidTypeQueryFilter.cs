using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class LiquidTypeQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? NameContains { get; private set; }

        public LiquidTypeQueryFilter(
            int? id = null,
            string? name = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
        {
            Id = id;
            NameContains = name;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
            SortBy = sortBy;
            SortDesc = sortDesc;
            Page = page;
            PageSize = pageSize;
        }
    }
}
