using Financial.Domain.Enums;
using FluentAssertions;

namespace Financial.UnitTests.Domain.Enums
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class CurrencyExtensionsTests
    {
        [Theory]
        [InlineData(Currency.USD, "$")]
        [InlineData(Currency.EUR, "€")]
        [InlineData(Currency.BRL, "R$")]
        [InlineData(Currency.GBP, "£")]
        [InlineData(Currency.JPY, "¥")]
        [InlineData(Currency.CNY, "¥")]
        [InlineData(Currency.AUD, "A$")]
        [InlineData(Currency.CAD, "C$")]
        [InlineData(Currency.CHF, "CHF")]
        [InlineData(Currency.INR, "₹")]
        public void ToSymbol_WithKnownCurrency_ShouldReturnCorrectSymbol(Currency currency, string expectedSymbol)
        {
            // Act & Assert
            currency.ToSymbol().Should().Be(expectedSymbol);
        }

        [Fact]
        public void ToSymbol_WithUnmappedCurrency_ShouldReturnEnumName()
        {
            // Act
            var symbol = Currency.MXN.ToSymbol();

            // Assert
            symbol.Should().Be("MXN");
        }

        [Fact]
        public void ToSymbol_AllDefinedValues_ShouldNotThrow()
        {
            // Act & Assert
            foreach (Currency currency in Enum.GetValues<Currency>())
            {
                var act = () => currency.ToSymbol();
                act.Should().NotThrow();
            }
        }

        [Fact]
        public void ToSymbol_USD_ShouldReturnDollarSign()
        {
            // Act & Assert
            Currency.USD.ToSymbol().Should().Be("$");
        }

        [Fact]
        public void ToSymbol_BRL_ShouldReturnRealSign()
        {
            // Act & Assert
            Currency.BRL.ToSymbol().Should().Be("R$");
        }
    }
}
