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
                .AddIf(filter.TotalCaloriesEquals.HasValue, mf => mf.TotalCalories == filter.TotalCaloriesEquals!.Value)
                .AddIf(filter.TotalCaloriesGreaterThan.HasValue, mf => mf.TotalCalories > filter.TotalCaloriesGreaterThan!.Value)
                .AddIf(filter.TotalCaloriesLessThan.HasValue, mf => mf.TotalCalories < filter.TotalCaloriesLessThan!.Value);

            return builder.Build();
        }
    }
}
