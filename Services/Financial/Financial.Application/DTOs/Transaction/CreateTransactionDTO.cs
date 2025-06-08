using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.Transaction
{
    public record CreateTransactionDTO(
        int UserId,
        int FinancialAccountId,
        int? CategoryId,
        TransactionType Type,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        bool IsRecurring = false);
}
