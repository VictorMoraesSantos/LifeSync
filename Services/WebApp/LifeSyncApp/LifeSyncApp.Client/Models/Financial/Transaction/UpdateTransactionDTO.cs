namespace LifeSyncApp.Client.Models.Financial.Transaction
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