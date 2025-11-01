using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class LiquidQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? NameContains { get; private set; }
        public int? QuantityMlEquals { get; private set; }
        public int? QuantityMlGreaterThan { get; private set; }
        public int? QuantityMlLessThan { get; private set; }
        public int? CaloriesPerMlEquals { get; private set; }
        public int? CaloriesPerMlGreaterThan { get; private set; }
        public int? CaloriesPerMlLessTaen { get; private set; }
        public int? DiaryId { get; private set; }
        public int? TotalCaloriesEquals { get; private set; }
        public int? TotalCaloriesGreaterThan { get; private set; }
        public int? TotalCaloriesLessThan { get; private set; }

        public LiquidQueryFilter(
            int? id = null,
            string? name = null,
            int? quantityMlEquals = null,
            int? quantityMlGreaterThan = null,
            int? quantityMlLessThan = null,
            int? caloriesPerMl = null,
            int? caloriesPerMlGreaterThan = null,
            int? caloriesPerMlLessThan = null,
            int? diaryId = null,
            int? totalCaloriesEquals = null,
            int? totalCaloriesGreaterThan = null,
            int? totalCaloriesLessThan = null,
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
            QuantityMlEquals = quantityMlEquals;
            QuantityMlGreaterThan = quantityMlGreaterThan;
            QuantityMlLessThan = quantityMlLessThan;
            CaloriesPerMlEquals = caloriesPerMl;
            CaloriesPerMlGreaterThan = caloriesPerMlGreaterThan;
            CaloriesPerMlLessTaen = caloriesPerMlLessThan;
            DiaryId = diaryId;
            TotalCaloriesEquals = totalCaloriesEquals;
            TotalCaloriesGreaterThan = totalCaloriesGreaterThan;
            TotalCaloriesLessThan = totalCaloriesLessThan;
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
