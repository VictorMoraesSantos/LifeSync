namespace Financial.Domain.Enums
{
    public enum TransactionType
    {
        Income = 1,
        Expense = 2
    }

    public static class TransactionTypeExtensions
    {
        public static string ToFriendlyString(this TransactionType transactionType)
        {
            return transactionType switch
            {
                TransactionType.Income => "Renda",
                TransactionType.Expense => "Despesa",
                _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
            };
        }
    }
}
