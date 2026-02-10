namespace LifeSyncApp.Models.Financial
{
    public class Money
    {
        public int Amount { get; set; }
        public Currency Currency { get; set; }

        public Money()
        {
            Currency = Currency.BRL;
        }

        public Money(int amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal ToDecimal()
        {
            return Amount / 100m;
        }

        public static Money FromDecimal(decimal value, Currency currency = Currency.BRL)
        {
            return new Money((int)(value * 100), currency);
        }

        public string ToFormattedString()
        {
            var symbol = Currency.ToSymbol();
            var value = ToDecimal();
            return $"{symbol} {value:N2}";
        }

        public override string ToString()
        {
            return ToFormattedString();
        }
    }
}
