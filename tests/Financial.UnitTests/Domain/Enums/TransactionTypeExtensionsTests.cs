using Financial.Domain.Enums;
using FluentAssertions;

namespace Financial.UnitTests.Domain.Enums
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class TransactionTypeExtensionsTests
    {
        [Fact]
        public void ToFriendlyString_Income_ShouldReturnRenda()
        {
            // Act & Assert
            TransactionType.Income.ToFriendlyString().Should().Be("Renda");
        }

        [Fact]
        public void ToFriendlyString_Expense_ShouldReturnDespesa()
        {
            // Act & Assert
            TransactionType.Expense.ToFriendlyString().Should().Be("Despesa");
        }

        [Fact]
        public void ToFriendlyString_InvalidValue_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var invalidType = (TransactionType)999;

            // Act
            var act = () => invalidType.ToFriendlyString();

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
