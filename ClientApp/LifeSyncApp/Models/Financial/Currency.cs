namespace LifeSyncApp.Models.Financial
{
    public enum Currency
    {
        BRL = 1,
        USD = 2,
        EUR = 3
    }

    public static class CurrencyExtensions
    {
        public static string ToSymbol(this Currency currency)
        {
            return currency switch
            {
                Currency.BRL => "R$",
                Currency.USD => "$",
                Currency.EUR => "€",
                _ => currency.ToString()
            };
        }
    }
}
