namespace LifeSyncApp.Client.Models.Financial.Transaction
{
    public enum PaymentMethod { Cash = 1, CreditCard, DebitCard, BankTransfer, Pix, Other }
    public enum TransactionType { Income = 1, Expense = 2 }
    public enum Currency { USD, EUR, BRL, GBP, JPY, CNY, AUD, CAD, CHF, INR }

    public record Money(int Amount, Currency Currency);
}
