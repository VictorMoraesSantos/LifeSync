using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class DailyProgressQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public DateOnly? Date { get; private set; }
        public int? CaloriesConsumedEquals { get; private set; }
        public int? CaloriesConsumedGreaterThan { get; private set; }
        public int? CaloriesConsumedLessThan { get; private set; }
        public int? LiquidsConsumedMlEquals { get; private set; }
        public int? LiquidsConsumedMlGreaterThan { get; private set; }
        public int? LiquidsConsumedMlLessThan { get; private set; }

        public DailyProgressQueryFilter(
            int? id = null,
            int? userId = null,
            DateOnly? date = null,
            int? caloriesConsumedEquals = null,
            int? caloriesConsumedGreaterThan = null,
            int? caloriesConsumedLessThan = null,
            int? liquidsConsumedMlEquals = null,
            int? liquidsConsumedMlGreaterThan = null,
            int? liquidsConsumedMlLessThan = null,
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
            Date = date;
            CaloriesConsumedEquals = caloriesConsumedEquals;
            CaloriesConsumedGreaterThan = caloriesConsumedGreaterThan;
            CaloriesConsumedLessThan = caloriesConsumedLessThan;
            LiquidsConsumedMlEquals = liquidsConsumedMlEquals;
            LiquidsConsumedMlGreaterThan = liquidsConsumedMlGreaterThan;
            LiquidsConsumedMlLessThan = liquidsConsumedMlLessThan;
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
