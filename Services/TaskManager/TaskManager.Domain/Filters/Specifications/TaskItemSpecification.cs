using Core.Domain.Filters;
using System.Linq.Expressions;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Filters.Specifications
{
    public class TaskItemSpecification : BaseFilterSpecification<TaskItem, int>
    {
        public TaskItemSpecification(TaskItemQueryFilter filter)
            : base(filter, BuildCriteria, ConfigureIncludes)
        { }

        private static Expression<Func<TaskItem, bool>>? BuildCriteria(IDomainQueryFilter baseFilter)
        {
            var filter = (TaskItemQueryFilter)baseFilter;
            var builder = new FilterCriteriaBuilder<TaskItem, int>(filter)
                .AddCommonFilters()
                .AddIf(filter.Id.HasValue, t => t.Id == filter.Id!.Value)
                .AddIf(filter.UserId.HasValue, t => t.UserId == filter.UserId!.Value)
                .AddIf(!string.IsNullOrWhiteSpace(filter.TitleContains), t => t.Title.Contains(filter.TitleContains!))
                .AddIf(filter.Status.HasValue, t => t.Status == filter.Status!.Value)
                .AddIf(filter.Priority.HasValue, t => t.Priority == filter.Priority!.Value)
                .AddIf(filter.DueDate.HasValue, t => t.DueDate == filter.DueDate!.Value)
                .AddIf(filter.LabelId.HasValue, t => t.Labels.Any(l => l.Id == filter.LabelId!.Value));

            return builder.Build();
        }

        private static void ConfigureIncludes(BaseFilterSpecification<TaskItem, int> spec)
        {
            spec.AddInclude(t => t.Labels);
        }
    }
}
