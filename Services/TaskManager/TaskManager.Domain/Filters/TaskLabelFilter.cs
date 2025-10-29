using Core.Domain.Filters;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Filters
{
    public class TaskLabelFilter : DomainQueryFilter<int>
    {
        public int? UserId { get; }
        public int? TaskItemId { get; }
        public string? NameContains { get; }
        public LabelColor? LabelColor { get; }

        public TaskLabelFilter(
            int? userId = null,
            int? taskItemId = null,
            string? nameContains = null,
            LabelColor? labelColor = null,
            int? id = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
            : base(id ?? default, createdAt, updatedAt, isDeleted, sortBy, sortDesc, page, pageSize)
        {
            UserId = userId;
            TaskItemId = taskItemId;
            NameContains = nameContains;
            LabelColor = labelColor;
        }
    }
}
