using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.DTOs.Transaction
{
    public record UpdateTransactionDTO(
        int Id,
        int? CategoryId,
        PaymentMethod PaymentMethod,
        TransactionType TransactionType,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        bool IsRecurring = false);
}
