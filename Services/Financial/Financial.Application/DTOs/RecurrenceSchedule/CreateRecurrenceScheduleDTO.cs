using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record CreateRecurrenceScheduleDTO(
        int TransactionId,
        RecurrenceFrequency Frequency,
        DateTime StartDate,
        DateTime? EndDate = null,
        int? MaxOccurrences = null);
}
