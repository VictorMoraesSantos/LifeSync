using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.Transaction
{
    public record CreateTransactionDTO(
        int UserId,
        int FinancialAccountId,
        TransactionType Type,
        Money Amount,
        string Currency,
        string Description,
        DateTime TransactionDate,
        int? CategoryId,
        bool IsRecurring = false);
}
