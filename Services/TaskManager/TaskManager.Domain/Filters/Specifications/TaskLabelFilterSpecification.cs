using Core.Domain.Filters;
using System.Linq.Expressions;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Filters.Specifications
{
    public class TaskLabelFilterSpecification : BaseFilterSpecification<TaskLabel, int>
    {
        public TaskLabelFilterSpecification(TaskLabelFilter filter)
            : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<TaskLabel, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (TaskLabelFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<TaskLabel, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, tl => tl.Id == filter.Id.Value)
                .AddIf(filter.UserId.HasValue, tl => tl.UserId == filter.UserId.Value)
                .AddIf(filter.TaskItemId.HasValue, tl => tl.TaskItemId == filter.TaskItemId.Value)
                .AddIf(!string.IsNullOrEmpty(filter.NameContains), tl => tl.Name.Contains(filter.NameContains!))
                .AddIf(filter.LabelColor.HasValue, tl => tl.LabelColor == filter.LabelColor.Value);

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<TaskLabel, int> spec)
        {
            spec.AddInclude(tl => tl.TaskItem);
        }
    }
}
