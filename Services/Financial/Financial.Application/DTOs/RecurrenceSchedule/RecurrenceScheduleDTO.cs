using Core.Application.DTO;
using Financial.Application.DTOs.Transaction;
using Financial.Domain.Enums;

namespace Financial.Application.DTOs.RecurrenceSchedule
{
    public record RecurrenceScheduleDTO(
        int Id,
        int TransactionId,
        TransactionDTO transaction,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        RecurrenceFrequency Frequency,
        DateTime StartDate,
        DateTime EndDate,
        DateTime NextOccurrence,
        int? MaxOccurrences,
        int OccurrencesGenerated,
        bool IsActive)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
