using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class MealFoodQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? NameContains { get; private set; }
        public int? Quantity { get; private set; }
        public int? CaloriesPerUnitEqual { get; private set; }
        public int? CaloriesPerUnitGreaterThen { get; private set; }
        public int? CaloriesPerUnitLessThen { get; private set; }
        public int? MealId { get; private set; }
        public int? TotalCaloriesEqual { get; private set; }
        public int? TotalCaloriesGreaterThen { get; private set; }
        public int? TotalCaloriesLessThen { get; private set; }

        public MealFoodQueryFilter(
            int? id = null,
            string? nameContains = null,
            int? quantity = null,
            int? caloriesPerUnitEqual = null,
            int? caloriesPerUnitGreaterThen = null,
            int? caloriesPerUnitLessThen = null,
            int? mealId = null,
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
            NameContains = nameContains;
            Quantity = quantity;
            CaloriesPerUnitEqual = caloriesPerUnitEqual;
            CaloriesPerUnitGreaterThen = caloriesPerUnitGreaterThen;
            CaloriesPerUnitLessThen = caloriesPerUnitLessThen;
            MealId = mealId;
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
