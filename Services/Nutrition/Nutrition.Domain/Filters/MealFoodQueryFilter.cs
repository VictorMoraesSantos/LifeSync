using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class MealFoodQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? NameContains { get; private set; }
        public int? Quantity { get; private set; }
        public int? CaloriesPerUnitEquals { get; private set; }
        public int? CaloriesPerUnitGreaterThan { get; private set; }
        public int? CaloriesPerUnitLessThan { get; private set; }
        public int? MealId { get; private set; }
        public int? TotalCaloriesEquals { get; private set; }
        public int? TotalCaloriesGreaterThan { get; private set; }
        public int? TotalCaloriesLessThan { get; private set; }

        public MealFoodQueryFilter(
            int? id = null,
            string? nameContains = null,
            int? quantity = null,
            int? caloriesPerUnitEquals = null,
            int? caloriesPerUnitGreaterThan = null,
            int? caloriesPerUnitLessThan = null,
            int? mealId = null,
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
            NameContains = nameContains;
            Quantity = quantity;
            CaloriesPerUnitEquals = caloriesPerUnitEquals;
            CaloriesPerUnitGreaterThan = caloriesPerUnitGreaterThan;
            CaloriesPerUnitLessThan = caloriesPerUnitLessThan;
            MealId = mealId;
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
