namespace LifeSyncApp.Models.Financial
{
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
                PaymentMethod.BankTransfer => "Transferência",
                PaymentMethod.Pix => "PIX",
                PaymentMethod.Other => "Outro",
                _ => method.ToString()
            };
        }
    }
}
