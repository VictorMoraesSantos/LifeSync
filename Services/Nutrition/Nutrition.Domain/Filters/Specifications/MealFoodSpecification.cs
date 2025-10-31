using Core.Domain.Filters;
using Nutrition.Domain.Entities;
using System.Linq.Expressions;

namespace Nutrition.Domain.Filters.Specifications
{
    public class MealFoodSpecification : BaseFilterSpecification<MealFood, int>
    {
        public MealFoodSpecification(MealFoodQueryFilter filter)
            : base(filter, BuildCriteria)
        { }

        private static Expression<Func<MealFood, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (MealFoodQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<MealFood, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, mf => mf.Id == filter.Id!.Value)
                .AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), mf => mf.Name.Contains(filter.NameContains!))
                .AddIf(filter.Quantity.HasValue, mf => mf.Quantity == filter.Quantity!.Value)
                .AddIf(filter.CaloriesPerUnitEqual.HasValue, mf => mf.CaloriesPerUnit == filter.CaloriesPerUnitEqual!.Value)
                .AddIf(filter.CaloriesPerUnitGreaterThen.HasValue, mf => mf.CaloriesPerUnit > filter.CaloriesPerUnitGreaterThen!.Value)
                .AddIf(filter.CaloriesPerUnitLessThen.HasValue, mf => mf.CaloriesPerUnit < filter.CaloriesPerUnitLessThen!.Value)
                .AddIf(filter.MealId.HasValue, mf => mf.MealId == filter.MealId!.Value)
                .AddIf(filter.TotalCaloriesEqual.HasValue, mf => mf.TotalCalories == filter.TotalCaloriesEqual!.Value)
                .AddIf(filter.TotalCaloriesGreaterThen.HasValue, mf => mf.TotalCalories > filter.TotalCaloriesGreaterThen!.Value)
                .AddIf(filter.TotalCaloriesLessThen.HasValue, mf => mf.TotalCalories < filter.TotalCaloriesLessThen!.Value);

            return builder.Build();
        }
    }
}
