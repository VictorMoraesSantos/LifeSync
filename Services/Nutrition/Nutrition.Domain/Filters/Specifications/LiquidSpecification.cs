using Core.Domain.Filters;
using Nutrition.Domain.Entities;
using System.Linq.Expressions;

namespace Nutrition.Domain.Filters.Specifications
{
    public class LiquidSpecification : BaseFilterSpecification<Liquid, int>
    {
        public LiquidSpecification(LiquidQueryFilter filter)
            : base(filter, BuildCriteria)
        { }

        private static Expression<Func<Liquid, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (LiquidQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<Liquid, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, l => l.Id == filter.Id!.Value)
                .AddIf(!string.IsNullOrWhiteSpace(filter.NameContains), l => l.Name.Contains(filter.NameContains!))
                .AddIf(filter.QuantityMlEqual.HasValue, l => l.QuantityMl == filter.QuantityMlEqual!.Value)
                .AddIf(filter.QuantityMlGreaterThen.HasValue, l => l.QuantityMl > filter.QuantityMlGreaterThen!.Value)
                .AddIf(filter.QuantityMlLessThen.HasValue, l => l.QuantityMl < filter.QuantityMlLessThen!.Value)
                .AddIf(filter.CaloriesPerMlEqual.HasValue, l => l.CaloriesPerMl == filter.CaloriesPerMlEqual!.Value)
                .AddIf(filter.CaloriesPerMlGreaterThen.HasValue, l => l.CaloriesPerMl > filter.CaloriesPerMlGreaterThen!.Value)
                .AddIf(filter.CaloriesPerMlLessThen.HasValue, l => l.CaloriesPerMl < filter.CaloriesPerMlLessThen!.Value)
                .AddIf(filter.DiaryId.HasValue, l => l.DiaryId == filter.DiaryId!.Value)
                .AddIf(filter.TotalCaloriesEqual.HasValue, l => l.TotalCalories == filter.TotalCaloriesEqual!.Value)
                .AddIf(filter.TotalCaloriesGreaterThen.HasValue, l => l.TotalCalories > filter.TotalCaloriesGreaterThen!.Value)
                .AddIf(filter.TotalCaloriesLessThen.HasValue, l => l.TotalCalories < filter.TotalCaloriesLessThen!.Value);

            return builder.Build();
        }
    }
}
