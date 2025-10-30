using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class DiaryQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public int? TotalCaloriesEqual { get; private set; }
        public int? TotalCaloriesGreaterThen { get; private set; }
        public int? TotalCaloriesLessThen { get; private set; }
        public int? TotalLiquidsMlEqual { get; private set; }
        public int? TotalLiquidsMlGreaterThen { get; private set; }
        public int? TotalLiquidsMlLessThen { get; private set; }
        public int? MealId { get; private set; }
        public int? LiquidId { get; private set; }

        public DiaryQueryFilter(
            int? id = null,
            int? userId = null,
            int? totalCaloriesEqual = null,
            int? totalCaloriesGreaterThen = null,
            int? totalCaloriesLessThen = null,
            int? totalLiquidsMlEqual = null,
            int? totalLiquidsMlGreaterThen = null,
            int? totalLiquidsMlLessThen = null,
            int? mealId = null,
            int? liquidId = null,
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
            TotalCaloriesEqual = totalCaloriesEqual;
            TotalCaloriesGreaterThen = totalCaloriesGreaterThen;
            TotalCaloriesLessThen = totalCaloriesLessThen;
            TotalLiquidsMlEqual = totalLiquidsMlEqual;
            TotalLiquidsMlGreaterThen = totalLiquidsMlGreaterThen;
            TotalLiquidsMlLessThen = totalLiquidsMlLessThen;
            MealId = mealId;
            LiquidId = liquidId;
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
