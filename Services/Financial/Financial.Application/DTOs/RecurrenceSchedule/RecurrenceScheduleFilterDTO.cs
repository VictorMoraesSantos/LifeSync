using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record RecurrenceScheduleFilterDTO(
        int? Id = null,
        int? TransactionId = null,
        int? UserId = null,
        RecurrenceFrequency? Frequency = null,
        bool? IsActive = null,
        DateTime? StartDateFrom = null,
        DateTime? StartDateTo = null,
        DateTime? CreatedAt = null,
        DateTime? UpdatedAt = null,
        bool? IsDeleted = null,
        string? SortBy = null,
        bool? SortDesc = null,
        int? Page = null,
        int? PageSize = null);
}
