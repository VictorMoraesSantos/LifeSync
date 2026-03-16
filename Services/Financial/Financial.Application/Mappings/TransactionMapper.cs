using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.DTOs.Transaction;
using Financial.Domain.Entities;

namespace Financial.Application.Mappings
{
    public static class TransactionMapper
    {
        public static Transaction ToEntity(CreateTransactionDTO dto)
        {
            Transaction entity = new(
                dto.UserId,
                dto.CategoryId,
                dto.PaymentMethod,
                dto.TransactionType,
                dto.Amount,
                dto.Description,
                dto.TransactionDate,
                dto.IsRecurring);
            return entity;
        }

        public static TransactionDTO ToDTO(Transaction entity)
        {
            TransactionDTO dto = new(
                entity.Id,
                entity.UserId,
                entity.Category != null ? CategoryMapper.ToDTO(entity.Category) : null,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.PaymentMethod,
                entity.TransactionType,
                entity.Amount,
                entity.Description,
                entity.TransactionDate,
                entity.IsRecurring,
                entity.RecurrenceSchedule != null ? MapScheduleInfo(entity.RecurrenceSchedule) : null);
            return dto;
        }

        private static RecurrenceScheduleInfoDTO MapScheduleInfo(RecurrenceSchedule schedule)
        {
            return new RecurrenceScheduleInfoDTO(
                schedule.Id,
                schedule.Frequency,
                schedule.StartDate,
                schedule.EndDate,
                schedule.NextOccurrence,
                schedule.MaxOccurrences,
                schedule.OccurrencesGenerated,
                schedule.IsActive);
        }
    }
}
