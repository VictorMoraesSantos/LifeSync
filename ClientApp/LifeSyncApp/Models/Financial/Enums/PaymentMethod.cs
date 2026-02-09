namespace LifeSyncApp.Models.Financial.Enums;

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    DebitCard = 3,
    BankTransfer = 4,
    Pix = 5,
    Other = 6
}

public static class PaymentMethodExtensions
{
    public static string ToDisplayString(this PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Cash => "Dinheiro",
            PaymentMethod.CreditCard => "Cartão de Crédito",
            PaymentMethod.DebitCard => "Cartão de Débito",
            PaymentMethod.BankTransfer => "Transferência Bancária",
            PaymentMethod.Pix => "Pix",
            PaymentMethod.Other => "Outro",
            _ => method.ToString()
        };
    }

    public static string ToIcon(this PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Cash => "💵",
            PaymentMethod.CreditCard => "💳",
            PaymentMethod.DebitCard => "💳",
            PaymentMethod.BankTransfer => "🏦",
            PaymentMethod.Pix => "📱",
            PaymentMethod.Other => "📋",
            _ => "💰"
        };
    }
}
