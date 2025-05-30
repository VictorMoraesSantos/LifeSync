using Core.Application.DTO;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.Transaction
{
    public record TransactionDTO(
        int Id,
        int UserId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int FinancialAccountId,
        TransactionType Type,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        int? CategoryId,
        bool IsRecurring = false)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
