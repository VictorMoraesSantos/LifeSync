using Core.Domain.Filters;
using Financial.Domain.Entities;

namespace Financial.Domain.Filters.Specifications
{
    public class RecurrenceScheduleSpecification : Specification<RecurrenceSchedule, int>
    {
        public RecurrenceScheduleSpecification(RecurrenceScheduleQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, r => r.Id == filter.Id!.Value);
            AddIf(filter.TransactionId.HasValue, r => r.TransactionId == filter.TransactionId!.Value);
            AddIf(filter.UserId.HasValue, r => r.Transaction.UserId == filter.UserId!.Value);
            AddIf(filter.Frequency.HasValue, r => r.Frequency == filter.Frequency!.Value);
            AddIf(filter.IsActive.HasValue, r => r.IsActive == filter.IsActive!.Value);
            AddIf(filter.StartDateFrom.HasValue, r => r.StartDate >= filter.StartDateFrom!.Value);
            AddIf(filter.StartDateTo.HasValue, r => r.StartDate <= filter.StartDateTo!.Value);
        }
    }
}
