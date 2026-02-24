using Core.Domain.Filters;
using Nutrition.Domain.Entities;

namespace Nutrition.Domain.Filters.Specifications
{
    public class MealFoodSpecification : Specification<MealFood, int>
    {
        public MealFoodSpecification(MealFoodQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, mf => mf.Id == filter.Id!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), mf => mf.Food.Name.Contains(filter.NameContains!));
            AddIf(filter.Quantity.HasValue, mf => mf.Quantity == filter.Quantity!.Value);
            AddIf(filter.TotalCaloriesEquals.HasValue, mf => mf.TotalCalories == filter.TotalCaloriesEquals!.Value);
            AddIf(filter.TotalCaloriesGreaterThan.HasValue, mf => mf.TotalCalories > filter.TotalCaloriesGreaterThan!.Value);
            AddIf(filter.TotalCaloriesLessThan.HasValue, mf => mf.TotalCalories < filter.TotalCaloriesLessThan!.Value);
        }
    }
}
