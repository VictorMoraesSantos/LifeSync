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
                .AddIf(filter.QuantityMlEquals.HasValue, l => l.QuantityMl == filter.QuantityMlEquals!.Value)
                .AddIf(filter.QuantityMlGreaterThan.HasValue, l => l.QuantityMl > filter.QuantityMlGreaterThan!.Value)
                .AddIf(filter.QuantityMlLessThan.HasValue, l => l.QuantityMl < filter.QuantityMlLessThan!.Value)
                .AddIf(filter.CaloriesPerMlEquals.HasValue, l => l.CaloriesPerMl == filter.CaloriesPerMlEquals!.Value)
                .AddIf(filter.CaloriesPerMlGreaterThan.HasValue, l => l.CaloriesPerMl > filter.CaloriesPerMlGreaterThan!.Value)
                .AddIf(filter.CaloriesPerMlLessTaen.HasValue, l => l.CaloriesPerMl < filter.CaloriesPerMlLessTaen!.Value)
                .AddIf(filter.DiaryId.HasValue, l => l.DiaryId == filter.DiaryId!.Value)
                .AddIf(filter.TotalCaloriesEquals.HasValue, l => l.TotalCalories == filter.TotalCaloriesEquals!.Value)
                .AddIf(filter.TotalCaloriesGreaterThan.HasValue, l => l.TotalCalories > filter.TotalCaloriesGreaterThan!.Value)
                .AddIf(filter.TotalCaloriesLessThan.HasValue, l => l.TotalCalories < filter.TotalCaloriesLessThan!.Value);

            return builder.Build();
        }
    }
}
