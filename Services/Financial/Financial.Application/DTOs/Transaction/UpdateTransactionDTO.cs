using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.Transaction
{
    public record UpdateTransactionDTO(
        int Id,
        int UserId,
        TransactionType Type,
        Money Amount,
        string Currency,
        string Description,
        DateTime TransactionDate,
        int? CategoryId,
        bool IsRecurring = false);
}
