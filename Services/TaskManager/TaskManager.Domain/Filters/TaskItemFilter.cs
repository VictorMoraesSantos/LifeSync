using Core.Domain.Filters;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Filters
{
    public class TaskItemFilter : DomainQueryFilter<int>
    {
        public int? UserId { get; private set; }
        public string? TitleContains { get; private set; }
        public Status? Status { get; private set; }
        public Priority? Priority { get; private set; }
        public DateOnly? DueDate { get; private set; }
        public int? LabelId { get; private set; }

        public TaskItemFilter(
            int? userId = null,
            string? titleContains = null,
            Status? status = null,
            Priority? priority = null,
            DateOnly? dueDate = null,
            int? labelId = null,
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
            TitleContains = titleContains;
            Status = status;
            Priority = priority;
            DueDate = dueDate;
            LabelId = labelId;
        }
    }
}