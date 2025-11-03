using Core.Domain.Filters;

namespace Financial.Domain.Filters
{
    public class CategoryQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public string? NameContains { get; private set; }
        public string? DescriptionContains { get; private set; }

        public CategoryQueryFilter(
        int? id = null,
        int? userId = null,
        string? nameContains = null,
        string? descriptionContains = null,
        DateOnly? createdAt = null,
        DateOnly? updatedAt = null,
        bool? isDeleted = null,
        string? sortBy = null,
        bool? sortDesc = null,
        int? page = null,
        int? pageSize = null)
        {
            Id = id;
            UserId = userId;
            NameContains = nameContains;
            DescriptionContains = descriptionContains;
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
