using Financial.Domain.Enums;

namespace FinancialControl.Domain.ValueObjects
{
    public record Money
    {
        public int Amount { get; }
        public Currency Currency { get; }

        // Private constructor to enforce creation via factory for validation
        private Money(int amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(int amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentNullException(nameof(currency), "Currency cannot be empty.");
            if (currency.Length != 3)
                throw new ArgumentException("Currency code must be 3 characters long.", nameof(currency));

            if (!Enum.TryParse<Currency>(currency.ToUpperInvariant(), out var parsedCurrency))
                throw new ArgumentException($"Invalid currency code: {currency}", nameof(currency));

            return new Money(amount, parsedCurrency);
        }

        public static Money Zero(string currency = "BRL")
        {
            if (!Enum.TryParse<Currency>(currency.ToUpperInvariant(), out var parsedCurrency))
                throw new ArgumentException($"Invalid currency code: {currency}", nameof(currency));
            return new Money(0, parsedCurrency);
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add money of different currencies.");
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot subtract money of different currencies.");
            return new Money(Amount - other.Amount, Currency);
        }

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}