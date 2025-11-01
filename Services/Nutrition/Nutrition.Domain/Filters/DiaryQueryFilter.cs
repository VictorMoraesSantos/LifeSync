using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class DiaryQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public int? TotalCaloriesEquals { get; private set; }
        public int? TotalCaloriesGreaterThan { get; private set; }
        public int? TotalCaloriesLessThan { get; private set; }
        public int? TotalLiquidsMlEquals { get; private set; }
        public int? TotalLiquidsMlGreaterThan { get; private set; }
        public int? TotalLiquidsMlLessThan { get; private set; }
        public int? MealId { get; private set; }
        public int? LiquidId { get; private set; }

        public DiaryQueryFilter(
            int? id = null,
            int? userId = null,
            int? totalCaloriesEquals = null,
            int? totalCaloriesGreaterThan = null,
            int? totalCaloriesLessThan = null,
            int? totalLiquidsMlEquals = null,
            int? totalLiquidsMlGreaterThan = null,
            int? totalLiquidsMlLessThan = null,
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
            TotalCaloriesEquals = totalCaloriesEquals;
            TotalCaloriesGreaterThan = totalCaloriesGreaterThan;
            TotalCaloriesLessThan = totalCaloriesLessThan;
            TotalLiquidsMlEquals = totalLiquidsMlEquals;
            TotalLiquidsMlGreaterThan = totalLiquidsMlGreaterThan;
            TotalLiquidsMlLessThan = totalLiquidsMlLessThan;
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
