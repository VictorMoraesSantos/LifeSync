using Core.Domain.Filters;
using Nutrition.Domain.Entities;

namespace Nutrition.Domain.Filters.Specifications
{
    public class DiarySpecification : Specification<Diary, int>
    {
        public DiarySpecification(DiaryQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, d => d.Id == filter.Id!.Value);
            AddIf(filter.UserId.HasValue, d => d.UserId == filter.UserId!.Value);
            AddIf(filter.TotalCaloriesEquals.HasValue, d => d.TotalCalories == filter.TotalCaloriesEquals!.Value);
            AddIf(filter.TotalCaloriesGreaterThan.HasValue, d => d.TotalCalories > filter.TotalCaloriesGreaterThan!.Value);
            AddIf(filter.TotalCaloriesLessThan.HasValue, d => d.TotalCalories < filter.TotalCaloriesLessThan!.Value);
            AddIf(filter.TotalLiquidsMlEquals.HasValue, d => d.TotalLiquidsMl == filter.TotalLiquidsMlEquals!.Value);
            AddIf(filter.TotalLiquidsMlGreaterThan.HasValue, d => d.TotalLiquidsMl > filter.TotalLiquidsMlGreaterThan!.Value);
            AddIf(filter.TotalLiquidsMlLessThan.HasValue, d => d.TotalLiquidsMl < filter.TotalLiquidsMlLessThan!.Value);
            AddIf(filter.MealId.HasValue, d => d.Meals.Any(m => m.Id == filter.MealId!.Value));
            AddIf(filter.LiquidId.HasValue, d => d.Liquids.Any(l => l.Id == filter.LiquidId!.Value));
            AddInclude(d => d.Meals);
            AddInclude(d => d.Liquids);
        }
    }
}
