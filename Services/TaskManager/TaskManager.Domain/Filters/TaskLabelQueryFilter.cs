using Core.Domain.Filters;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Filters
{
    public class TaskLabelQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public int? TaskItemId { get; private set; }
        public string? NameContains { get; private set; }
        public LabelColor? LabelColor { get; private set; }

        public TaskLabelQueryFilter(
            int? id = null,
            int? userId = null,
            int? taskItemId = null,
            string? nameContains = null,
            LabelColor? labelColor = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
        {
            Id = id;
            UserId = userId;
            TaskItemId = taskItemId;
            NameContains = nameContains;
            LabelColor = labelColor;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
            SortBy = sortBy;
            SortDesc = sortDesc;
            Page = page ?? 1;
            PageSize = pageSize ?? 50;
        }
    }
}
