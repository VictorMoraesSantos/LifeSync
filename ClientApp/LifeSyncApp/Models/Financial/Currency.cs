namespace LifeSyncApp.Models.Financial
{
    public enum Currency
    {
        USD = 0,
        EUR = 1,
        BRL = 2
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
