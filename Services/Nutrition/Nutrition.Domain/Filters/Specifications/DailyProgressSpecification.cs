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
                .AddIf(filter.CaloriesConsumedEqual.HasValue, dp => dp.CaloriesConsumed == filter.CaloriesConsumedEqual!.Value)
                .AddIf(filter.CaloriesConsumedGreaterThen.HasValue, dp => dp.CaloriesConsumed > filter.CaloriesConsumedGreaterThen!.Value)
                .AddIf(filter.CaloriesConsumedLessThen.HasValue, dp => dp.CaloriesConsumed < filter.CaloriesConsumedLessThen!.Value)
                .AddIf(filter.LiquidsConsumedMlEqual.HasValue, dp => dp.LiquidsConsumedMl == filter.LiquidsConsumedMlEqual!.Value)
                .AddIf(filter.LiquidsConsumedMlGreaterThen.HasValue, dp => dp.LiquidsConsumedMl > filter.LiquidsConsumedMlGreaterThen!.Value)
                .AddIf(filter.LiquidsConsumedMlLessThen.HasValue, dp => dp.LiquidsConsumedMl < filter.LiquidsConsumedMlLessThen!.Value);

            return builder.Build();
        }
    }
}
