using Core.Domain.Filters;
using Nutrition.Domain.Entities;
using System.Linq.Expressions;

namespace Nutrition.Domain.Filters.Specifications
{
    public class MealSpecification : BaseFilterSpecification<Meal, int>
    {
        public MealSpecification(MealQueryFilter filter)
            : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<Meal, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (MealQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<Meal, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, m => m.Id == filter.Id!.Value)
                .AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), m => m.Name.Contains(filter.NameContains!))
                .AddIf(!string.IsNullOrWhiteSpace(filter.DescriptionContains), m => m.Description.Contains(filter.DescriptionContains!))
                .AddIf(filter.DiaryId.HasValue, m => m.DiaryId == filter.DiaryId!.Value)
                .AddIf(filter.TotalCaloriesEqual.HasValue, m => m.TotalCalories == filter.TotalCaloriesEqual!.Value)
                .AddIf(filter.TotalCaloriesGreaterThen.HasValue, m => m.TotalCalories > filter.TotalCaloriesGreaterThen!.Value)
                .AddIf(filter.TotalCaloriesLessThen.HasValue, m => m.TotalCalories < filter.TotalCaloriesLessThen!.Value)
                .AddIf(filter.MealFoodId.HasValue, m => m.MealFoods.Any(mf => mf.Id == filter.MealFoodId!.Value));

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<Meal, int> spec)
        {
            spec.AddInclude(m => m.MealFoods);
        }
    }
}
