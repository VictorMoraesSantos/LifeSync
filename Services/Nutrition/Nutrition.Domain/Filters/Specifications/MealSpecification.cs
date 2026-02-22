using Core.Domain.Filters;
using Nutrition.Domain.Entities;

namespace Nutrition.Domain.Filters.Specifications
{
    public class MealSpecification : Specification<Meal, int>
    {
        public MealSpecification(MealQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, m => m.Id == filter.Id!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), m => m.Name.Contains(filter.NameContains!));
            AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), m => m.Description.Contains(filter.DescriptionContains!));
            AddIf(filter.DiaryId.HasValue, m => m.DiaryId == filter.DiaryId!.Value);
            AddIf(filter.TotalCaloriesEqual.HasValue, m => m.TotalCalories == filter.TotalCaloriesEqual!.Value);
            AddIf(filter.TotalCaloriesGreaterThen.HasValue, m => m.TotalCalories > filter.TotalCaloriesGreaterThen!.Value);
            AddIf(filter.TotalCaloriesLessThen.HasValue, m => m.TotalCalories < filter.TotalCaloriesLessThen!.Value);
            AddIf(filter.MealFoodId.HasValue, m => m.MealFoods.Any(mf => mf.Id == filter.MealFoodId!.Value));
            AddInclude(m => m.MealFoods);
        }
    }
}
