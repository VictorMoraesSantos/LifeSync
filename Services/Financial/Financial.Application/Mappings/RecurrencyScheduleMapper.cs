using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Domain.Entities;

namespace Financial.Application.Mappings
{
    public static class RecurrencyScheduleMapper
    {
        public static RecurrenceSchedule ToEntity(CreateRecurrenceScheduleDTO dto)
        {
            RecurrenceSchedule entity = new RecurrenceSchedule(
                dto.TransactionId,
                dto.Frequency,
                dto.StartDate,
                dto.EndDate,
                dto.MaxOccurrences);
            return entity;
        }

        public static RecurrenceScheduleDTO ToDTO(RecurrenceSchedule entity)
        {
            RecurrenceScheduleDTO dto = new RecurrenceScheduleDTO(
                entity.Id,
                entity.TransactionId,
                TransactionMapper.ToDTO(entity.Transaction),
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Frequency,
                entity.StartDate,
                entity.EndDate,
                entity.NextOccurrence,
                entity.MaxOccurrences,
                entity.OccurrencesGenerated,
                entity.IsActive);
            return dto;
        }
    }
}
