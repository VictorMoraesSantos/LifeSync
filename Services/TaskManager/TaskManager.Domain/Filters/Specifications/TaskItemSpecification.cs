using Core.Domain.Filters;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Filters.Specifications
{
    public class TaskItemSpecification : Specification<TaskItem, int>
    {
        public TaskItemSpecification(TaskItemQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, t => t.Id == filter.Id!.Value);
            AddIf(filter.UserId.HasValue, t => t.UserId == filter.UserId!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.TitleContains), t => t.Title.Contains(filter.TitleContains!));
            AddIf(filter.Status.HasValue, t => t.Status == filter.Status!.Value);
            AddIf(filter.Priority.HasValue, t => t.Priority == filter.Priority!.Value);
            AddIf(filter.DueDate.HasValue, t => t.DueDate == filter.DueDate!.Value);
            AddIf(filter.LabelId.HasValue, t => t.Labels.Any(l => l.Id == filter.LabelId!.Value));
            AddInclude(t => t.Labels);
        }
    }
}
