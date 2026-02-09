namespace LifeSyncApp.Models.Financial.Enums;

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

    public static Color ToColor(this TransactionType type)
    {
        return type switch
        {
            TransactionType.Income => Color.FromArgb("#10B981"), // Verde
            TransactionType.Expense => Color.FromArgb("#EF4444"), // Vermelho
            _ => Colors.Gray
        };
    }

    public static string ToIcon(this TransactionType type)
    {
        return type switch
        {
            TransactionType.Income => "↑", // Seta para cima
            TransactionType.Expense => "↓", // Seta para baixo
            _ => "•"
        };
    }
}
