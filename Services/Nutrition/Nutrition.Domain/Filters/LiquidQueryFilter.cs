using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class LiquidQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? DiaryId { get; private set; }
        public string? NameContains { get; private set; }
        public int? QuantityEquals { get; private set; }
        public int? QuantityGreaterThan { get; private set; }
        public int? QuantityLessThan { get; private set; }

        public LiquidQueryFilter(
            int? id = null,
            int? diaryId = null,
            string? name = null,
            int? quantityEquals = null,
            int? quantityGreaterThan = null,
            int? quantityLessThan = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
        {
            Id = id;
            DiaryId = diaryId;
            NameContains = name;
            QuantityEquals = quantityEquals;
            QuantityGreaterThan = quantityGreaterThan;
            QuantityLessThan = quantityLessThan;
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
