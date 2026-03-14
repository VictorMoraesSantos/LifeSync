using Core.Domain.Filters;
using Financial.Domain.Enums;

namespace Financial.Domain.Filters
{
    public class RecurrenceScheduleQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? TransactionId { get; private set; }
        public int? UserId { get; private set; }
        public RecurrenceFrequency? Frequency { get; private set; }
        public bool? IsActive { get; private set; }
        public DateTime? StartDateFrom { get; private set; }
        public DateTime? StartDateTo { get; private set; }

        public RecurrenceScheduleQueryFilter(
            int? id = null,
            int? transactionId = null,
            int? userId = null,
            RecurrenceFrequency? frequency = null,
            bool? isActive = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
        {
            Id = id;
            TransactionId = transactionId;
            UserId = userId;
            Frequency = frequency;
            IsActive = isActive;
            StartDateFrom = startDateFrom;
            StartDateTo = startDateTo;
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
