using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class LiquidQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? NameContains { get; private set; }
        public int? QuantityMlEqual { get; private set; }
        public int? QuantityMlGreaterThen { get; private set; }
        public int? QuantityMlLessThen { get; private set; }
        public int? CaloriesPerMlEqual { get; private set; }
        public int? CaloriesPerMlGreaterThen { get; private set; }
        public int? CaloriesPerMlLessThen { get; private set; }
        public int? DiaryId { get; private set; }
        public int? TotalCaloriesEqual { get; private set; }
        public int? TotalCaloriesGreaterThen { get; private set; }
        public int? TotalCaloriesLessThen { get; private set; }

        public LiquidQueryFilter(
            int? id = null,
            string? name = null,
            int? quantityMlEqual = null,
            int? quantityMlGreaterThen = null,
            int? quantityMlLessThen = null,
            int? caloriesPerMl = null,
            int? caloriesPerMlGreaterThen = null,
            int? caloriesPerMlLessThen = null,
            int? diaryId = null,
            int? totalCaloriesEqual = null,
            int? totalCaloriesGreaterThen = null,
            int? totalCaloriesLessThen = null,
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
            QuantityMlEqual = quantityMlEqual;
            QuantityMlGreaterThen = quantityMlGreaterThen;
            QuantityMlLessThen = quantityMlLessThen;
            CaloriesPerMlEqual = caloriesPerMl;
            CaloriesPerMlGreaterThen = caloriesPerMlGreaterThen;
            CaloriesPerMlLessThen = caloriesPerMlLessThen;
            DiaryId = diaryId;
            TotalCaloriesEqual = totalCaloriesEqual;
            TotalCaloriesGreaterThen = totalCaloriesGreaterThen;
            TotalCaloriesLessThen = totalCaloriesLessThen;
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
