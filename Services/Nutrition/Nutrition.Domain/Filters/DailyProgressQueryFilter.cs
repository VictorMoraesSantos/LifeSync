using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class DailyProgressQueryFilter : IDomainQueryFilter<int?>
    {
        public int? Id { get; private set; }
        public DateTime? CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool? IsDeleted { get; private set; }
        public int? UserId { get; private set; }
        public DateOnly? Date { get; private set; } = default;
        public DateOnly? DateGreaterThen { get; set; } = default;
        public DateOnly? DateLessThen { get; set; } = default;
        public int? CaloriesConsumed { get; set; }
        public int? MinCaloriesConsumed { get; set; }
        public int? MaxCaloriesConsumed { get; set; }
        public int? LiquidsConsumedMl { get; set; }
        public int? MinLiquidsConsumedMl { get; set; }
        public int? MaxLiquidsConsumedMl { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; } = false;
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 50;
    }
}
