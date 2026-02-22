using Core.Domain.Filters;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Filters.Specifications
{
    public class TaskLabelSpecification : Specification<TaskLabel, int>
    {
        public TaskLabelSpecification(TaskLabelQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, tl => tl.Id == filter.Id!.Value);
            AddIf(filter.UserId.HasValue, tl => tl.UserId == filter.UserId!.Value);
            AddIf(filter.TaskItemId.HasValue, tl => tl.Items.Any(item => item.Id == filter.TaskItemId!.Value));
            AddIf(!string.IsNullOrEmpty(filter.NameContains), tl => tl.Name.Contains(filter.NameContains!));
            AddIf(filter.LabelColor.HasValue, tl => tl.LabelColor == filter.LabelColor!.Value);
            AddInclude(tl => tl.Items);
        }
    }
}
