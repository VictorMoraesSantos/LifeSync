using Core.Domain.Filters;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Filters
{
    public class TaskItemQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public string? TitleContains { get; private set; }
        public Status? Status { get; private set; }
        public Priority? Priority { get; private set; }
        public DateOnly? DueDate { get; private set; }
        public int? LabelId { get; private set; }

        public TaskItemQueryFilter(
            int? id = null,
            int? userId = null,
            string? titleContains = null,
            Status? status = null,
            Priority? priority = null,
            DateOnly? dueDate = null,
            int? labelId = null,
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
            TitleContains = titleContains;
            Status = status;
            Priority = priority;
            DueDate = dueDate;
            LabelId = labelId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
            SortBy = sortBy;
            SortDesc = sortDesc;
            Page = page;
            PageSize = pageSize;
        }
    }
}