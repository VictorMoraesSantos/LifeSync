using Financial.Domain.Enums;
using System.Text.Json.Serialization;

namespace FinancialControl.Domain.ValueObjects
{
    public record Money
    {
        public int Amount { get; } = default!;
        public Currency Currency { get; } = default!;

        protected Money()
        {
        }

        [JsonConstructor]
        private Money(int amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(int amount, Currency currency)
        {
            if (!Enum.IsDefined(typeof(Currency), currency))
                throw new ArgumentException($"Invalid currency value: {currency}", nameof(currency));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

            return new Money(amount, currency);
        }
    }
}