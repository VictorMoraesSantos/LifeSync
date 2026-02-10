namespace LifeSyncApp.Models.Financial
{
    public enum TransactionType
    {
        Income = 1,
        Expense = 2
    }

    public static class TransactionTypeExtensions
    {
        public static string ToDisplayString(this TransactionType type)
        {
            return type switch
            {
                TransactionType.Income => "Receita",
                TransactionType.Expense => "Despesa",
                _ => type.ToString()
            };
        }
    }
}
