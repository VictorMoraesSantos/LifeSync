using Core.Domain.Filters;
using Nutrition.Domain.Entities;
using System.Linq.Expressions;

namespace Nutrition.Domain.Filters.Specifications
{
    public class DiarySpecification : BaseFilterSpecification<Diary, int>
    {
        public DiarySpecification(DiaryQueryFilter filter)
            : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<Diary, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (DiaryQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<Diary, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, d => d.Id == filter.Id!.Value)
                .AddIf(filter.UserId.HasValue, d => d.UserId == filter.UserId!.Value)
                .AddIf(filter.TotalCaloriesEqual.HasValue, d => d.TotalCalories == filter.TotalCaloriesEqual!.Value)
                .AddIf(filter.TotalCaloriesGreaterThen.HasValue, d => d.TotalCalories > filter.TotalCaloriesGreaterThen!.Value)
                .AddIf(filter.TotalCaloriesLessThen.HasValue, d => d.TotalCalories < filter.TotalCaloriesLessThen!.Value)
                .AddIf(filter.TotalLiquidsMlEqual.HasValue, d => d.TotalLiquidsMl == filter.TotalLiquidsMlEqual!.Value)
                .AddIf(filter.TotalLiquidsMlGreaterThen.HasValue, d => d.TotalLiquidsMl > filter.TotalLiquidsMlGreaterThen!.Value)
                .AddIf(filter.TotalLiquidsMlLessThen.HasValue, d => d.TotalLiquidsMl < filter.TotalLiquidsMlLessThen!.Value)
                .AddIf(filter.MealId.HasValue, d => d.Meals.Any(m => m.Id == filter.MealId!.Value))
                .AddIf(filter.LiquidId.HasValue, d => d.Liquids.Any(l => l.Id == filter.LiquidId!.Value));

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<Diary, int> spec)
        {
            spec.AddInclude(d => d.Meals);
            spec.AddInclude(d => d.Liquids);
        }
    }
}
