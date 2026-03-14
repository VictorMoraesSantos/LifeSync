using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.DTOs.Financial.Transaction
{
    public record CreateTransactionDTO(
        int UserId,
        int? CategoryId,
        PaymentMethod PaymentMethod,
        TransactionType TransactionType,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        bool IsRecurring = false,
        RecurrenceFrequency? Frequency = null,
        DateTime? RecurrenceEndDate = null,
        int? MaxOccurrences = null);
}
