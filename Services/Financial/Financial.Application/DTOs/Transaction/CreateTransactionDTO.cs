using Financial.Domain.Enums;

namespace Financial.Application.DTOs.Transaction
{
    public record CreateTransactionDTO(
        int FinancialAccountId,
        TransactionType Type,
        int Amount,
        string Currency,
        string Description,
        DateTime TransactionDate,
        int? CategoryId,
        bool IsRecurring = false);
}
