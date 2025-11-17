namespace LifeSyncApp.Client.Models.Financial.Transaction
{
    public class Money
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public Money()
        {
            Amount = 0m;
            Currency = Currency.BRL;
        }
    }
}
