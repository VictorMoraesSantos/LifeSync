using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record UpdateRecurrenceScheduleDTO(
        int Id,
        RecurrenceFrequency Frequency,
        DateTime? EndDate = null,
        int? MaxOccurrences = null);
}
