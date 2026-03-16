using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record RecurrenceScheduleInfoDTO(
        int Id,
        RecurrenceFrequency Frequency,
        DateTime StartDate,
        DateTime? EndDate,
        DateTime NextOccurrence,
        int? MaxOccurrences,
        int OccurrencesGenerated,
        bool IsActive);
}
