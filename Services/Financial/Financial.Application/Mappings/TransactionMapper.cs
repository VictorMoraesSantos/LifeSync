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
                dto.FinancialAccountId,
                dto.Type,
                dto.Amount,
                dto.Description,
                dto.TransactionDate,
                dto.CategoryId,
                dto.IsRecurring);
            return entity;
        }

        public static TransactionDTO ToDTO(Transaction entity)
        {
            TransactionDTO dto = new(
                entity.Id,
                entity.UserId,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.FinancialAccountId,
                entity.Type,
                entity.Amount,
                entity.Description,
                entity.TransactionDate,
                entity.CategoryId,
                entity.IsRecurring);
            return dto;
        }
    }
}
