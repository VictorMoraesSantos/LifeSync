using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class DailyProgressQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public DateOnly? Date { get; private set; }
        public int? CaloriesConsumedEqual { get; private set; }
        public int? CaloriesConsumedGreaterThen { get; private set; }
        public int? CaloriesConsumedLessThen { get; private set; }
        public int? LiquidsConsumedMlEqual { get; private set; }
        public int? LiquidsConsumedMlGreaterThen { get; private set; }
        public int? LiquidsConsumedMlLessThen { get; private set; }

        public DailyProgressQueryFilter(
            int? id = null,
            int? userId = null,
            DateOnly? date = null,
            int? caloriesConsumedEqual = null,
            int? caloriesConsumedGreaterThen = null,
            int? caloriesConsumedLessThen = null,
            int? liquidsConsumedMlEqual = null,
            int? liquidsConsumedMlGreaterThen = null,
            int? liquidsConsumedMlLessThen = null,
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
            CaloriesConsumedEqual = caloriesConsumedEqual;
            CaloriesConsumedGreaterThen = caloriesConsumedGreaterThen;
            CaloriesConsumedLessThen = caloriesConsumedLessThen;
            LiquidsConsumedMlEqual = liquidsConsumedMlEqual;
            LiquidsConsumedMlGreaterThen = liquidsConsumedMlGreaterThen;
            LiquidsConsumedMlLessThen = liquidsConsumedMlLessThen;
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
