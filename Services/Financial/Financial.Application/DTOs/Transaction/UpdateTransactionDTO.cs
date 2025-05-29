using Financial.Domain.Enums;

namespace Financial.Application.DTOs.Transaction
{
    public record UpdateTransactionDTO(
        int Id,
        TransactionType Type,
        int Amount,
        string Currency,
        string Description,
        DateTime TransactionDate,
        int? CategoryId,
        bool IsRecurring = false);
}
