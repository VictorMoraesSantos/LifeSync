namespace LifeSyncApp.Models.Financial.Enums;

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
            PaymentMethod.Pix => "PIX",
            PaymentMethod.Other => "Outro",
            _ => "Desconhecido"
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
            PaymentMethod.Pix => "💱",
            PaymentMethod.Other => "💼",
            _ => "❔"
        };
    }
}
