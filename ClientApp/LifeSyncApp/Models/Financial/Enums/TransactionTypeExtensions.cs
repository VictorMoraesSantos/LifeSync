namespace LifeSyncApp.Models.Financial.Enums;

public static class TransactionTypeExtensions
{
    public static string ToDisplayString(this TransactionType type)
    {
        return type switch
        {
            TransactionType.Income => "Receita",
            TransactionType.Expense => "Despesa",
            _ => "Desconhecido"
        };
    }

    public static Color ToColor(this TransactionType type)
    {
        return type switch
        {
            TransactionType.Income => Color.FromArgb("#10B981"),
            TransactionType.Expense => Color.FromArgb("#EF4444"),
            _ => Color.FromArgb("#6B7280")
        };
    }

    public static string ToIcon(this TransactionType type)
    {
        return type switch
        {
            TransactionType.Income => "↑",
            TransactionType.Expense => "↓",
            _ => "•"
        };
    }
}
