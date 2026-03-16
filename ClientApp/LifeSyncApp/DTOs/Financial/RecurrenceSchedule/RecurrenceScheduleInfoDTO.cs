using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.DTOs.Financial.RecurrenceSchedule
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
