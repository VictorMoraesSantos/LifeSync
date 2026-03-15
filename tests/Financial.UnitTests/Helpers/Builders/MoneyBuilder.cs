using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.UnitTests.Helpers.Builders;

public class MoneyBuilder
{
    private int _amount = 1000;
    private Currency _currency = Currency.BRL;

    public MoneyBuilder WithAmount(int amount)
    {
        _amount = amount;
        return this;
    }

    public MoneyBuilder WithCurrency(Currency currency)
    {
        _currency = currency;
        return this;
    }

    public Money Build()
    {
        return Money.Create(_amount, _currency);
    }

    public static MoneyBuilder Default() => new();
}
