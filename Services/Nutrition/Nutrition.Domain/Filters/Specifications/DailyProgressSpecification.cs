using Core.Domain.Filters;
using Nutrition.Domain.Entities;
using System.Linq.Expressions;

namespace Nutrition.Domain.Filters.Specifications
{
    public class DailyProgressSpecification : BaseFilterSpecification<DailyProgress, int>
    {
        public DailyProgressSpecification(DailyProgressQueryFilter filter)
            : base(filter, BuildCriteria)
        { }

        private static Expression<Func<DailyProgress, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (DailyProgressQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<DailyProgress, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, dp => dp.Id == filter.Id!.Value)
                .AddIf(filter.UserId.HasValue, dp => dp.UserId == filter.UserId!.Value)
                .AddIf(filter.Date.HasValue, dp => dp.Date == filter.Date!.Value)
                .AddIf(filter.CaloriesConsumedEquals.HasValue, dp => dp.CaloriesConsumed == filter.CaloriesConsumedEquals!.Value)
                .AddIf(filter.CaloriesConsumedGreaterThan.HasValue, dp => dp.CaloriesConsumed > filter.CaloriesConsumedGreaterThan!.Value)
                .AddIf(filter.CaloriesConsumedLessThan.HasValue, dp => dp.CaloriesConsumed < filter.CaloriesConsumedLessThan!.Value)
                .AddIf(filter.LiquidsConsumedMlEquals.HasValue, dp => dp.LiquidsConsumedMl == filter.LiquidsConsumedMlEquals!.Value)
                .AddIf(filter.LiquidsConsumedMlGreaterThan.HasValue, dp => dp.LiquidsConsumedMl > filter.LiquidsConsumedMlGreaterThan!.Value)
                .AddIf(filter.LiquidsConsumedMlLessThan.HasValue, dp => dp.LiquidsConsumedMl < filter.LiquidsConsumedMlLessThan!.Value);

            return builder.Build();
        }
    }
}
