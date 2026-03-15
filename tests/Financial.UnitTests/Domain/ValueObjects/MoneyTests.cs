using Financial.Domain.Enums;
using Financial.UnitTests.Helpers.Builders;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;

namespace Financial.UnitTests.Domain.ValueObjects
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class MoneyTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateMoney()
        {
            // Act
            var money = Money.Create(1000, Currency.BRL);

            // Assert
            money.Should().NotBeNull();
            money.Amount.Should().Be(1000);
            money.Currency.Should().Be(Currency.BRL);
        }

        [Fact]
        public void Create_WithZeroAmount_ShouldSucceed()
        {
            // Act
            var money = Money.Create(0, Currency.USD);

            // Assert
            money.Amount.Should().Be(0);
            money.Currency.Should().Be(Currency.USD);
        }

        [Fact]
        public void Create_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException()
        {
            // Act
            var act = () => Money.Create(-1, Currency.BRL);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Create_WithInvalidCurrency_ShouldThrowArgumentException()
        {
            // Act
            var act = () => Money.Create(100, (Currency)9999);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(Currency.USD)]
        [InlineData(Currency.EUR)]
        [InlineData(Currency.BRL)]
        [InlineData(Currency.GBP)]
        [InlineData(Currency.JPY)]
        public void Create_WithValidCurrencies_ShouldSucceed(Currency currency)
        {
            // Act
            var money = Money.Create(100, currency);

            // Assert
            money.Currency.Should().Be(currency);
        }

        [Fact]
        public void Equality_WithSameValues_ShouldBeEqual()
        {
            // Arrange
            var money1 = Money.Create(100, Currency.BRL);
            var money2 = Money.Create(100, Currency.BRL);

            // Assert
            money1.Should().Be(money2);
        }

        [Fact]
        public void Equality_WithDifferentAmount_ShouldNotBeEqual()
        {
            // Arrange
            var money1 = Money.Create(100, Currency.BRL);
            var money2 = Money.Create(200, Currency.BRL);

            // Assert
            money1.Should().NotBe(money2);
        }

        [Fact]
        public void Equality_WithDifferentCurrency_ShouldNotBeEqual()
        {
            // Arrange
            var money1 = Money.Create(100, Currency.BRL);
            var money2 = Money.Create(100, Currency.USD);

            // Assert
            money1.Should().NotBe(money2);
        }

        [Fact]
        public void MoneyBuilder_ShouldCreateValidMoney()
        {
            // Act
            var money = MoneyBuilder.Default().Build();

            // Assert
            money.Should().NotBeNull();
            money.Amount.Should().Be(1000);
            money.Currency.Should().Be(Currency.BRL);
        }
    }
}
