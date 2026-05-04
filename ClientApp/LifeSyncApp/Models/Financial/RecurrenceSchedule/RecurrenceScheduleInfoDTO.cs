using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.Models.Financial.RecurrenceSchedule
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
