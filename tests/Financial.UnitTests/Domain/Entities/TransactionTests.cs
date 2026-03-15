using Core.Domain.Exceptions;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Errors;
using Financial.UnitTests.Helpers.Builders;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;

namespace Financial.UnitTests.Domain.Entities
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Domain")]
    public class TransactionTests
    {
        #region Constructor Tests

        [Fact]
        public void Create_WithValidParameters_ShouldCreateEntity()
        {
            // Arrange
            var userId = 1;
            var categoryId = 10;
            var paymentMethod = PaymentMethod.CreditCard;
            var transactionType = TransactionType.Expense;
            var amount = Money.Create(100, Currency.USD);
            var description = "Test Transaction";
            var transactionDate = DateTime.UtcNow;

            // Act
            var transaction = new Transaction(
                userId, categoryId, paymentMethod, transactionType, amount, description, transactionDate, true);

            // Assert
            transaction.Should().NotBeNull();
            transaction.UserId.Should().Be(userId);
            transaction.CategoryId.Should().Be(categoryId);
            transaction.PaymentMethod.Should().Be(paymentMethod);
            transaction.TransactionType.Should().Be(transactionType);
            transaction.Amount.Should().Be(amount);
            transaction.Description.Should().Be(description);
            transaction.IsRecurring.Should().BeTrue();
        }

        [Fact]
        public void Create_WithNullCategoryId_ShouldCreateEntity()
        {
            // Act
            var transaction = TransactionBuilder.Default()
                .WithCategoryId(null)
                .Build();

            // Assert
            transaction.CategoryId.Should().BeNull();
        }

        [Fact]
        public void Create_WithDefaultIsRecurring_ShouldBeFalse()
        {
            // Act
            var transaction = TransactionBuilder.Default().Build();

            // Assert
            transaction.IsRecurring.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Create_WithInvalidUserId_ShouldThrowArgumentOutOfRangeException(int invalidUserId)
        {
            // Act
            var act = () => new Transaction(
                invalidUserId, 1, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), "Test", DateTime.UtcNow);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Create_WithNullAmount_ShouldThrowArgumentNullException()
        {
            // Act
            var act = () => new Transaction(
                1, 1, PaymentMethod.Cash, TransactionType.Income,
                null!, "Test", DateTime.UtcNow);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullDescription_ShouldThrowArgumentNullException()
        {
            // Act
            var act = () => new Transaction(
                1, 1, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), null!, DateTime.UtcNow);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithEmptyDescription_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new Transaction(
                1, 1, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), string.Empty, DateTime.UtcNow);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithWhitespaceDescription_ShouldThrowArgumentException()
        {
            // Act
            var act = () => new Transaction(
                1, 1, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), "   ", DateTime.UtcNow);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_ShouldConvertTransactionDateToUtc()
        {
            // Arrange
            var localTime = DateTime.Now;

            // Act
            var transaction = TransactionBuilder.Default()
                .WithTransactionDate(localTime)
                .Build();

            // Assert
            transaction.TransactionDate.Kind.Should().Be(DateTimeKind.Utc);
            transaction.TransactionDate.Should().Be(DateTime.SpecifyKind(localTime, DateTimeKind.Utc));
        }

        [Fact]
        public void Create_ShouldSetCreatedAt()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var transaction = TransactionBuilder.Default().Build();

            // Assert
            transaction.CreatedAt.Should().BeOnOrAfter(before);
            transaction.UpdatedAt.Should().BeNull();
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateProperties()
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();
            var newCategoryId = 20;
            var newPaymentMethod = PaymentMethod.DebitCard;
            var newTransactionType = TransactionType.Income;
            var newAmount = Money.Create(200, Currency.EUR);
            var newDescription = "Updated";
            var newDate = DateTime.UtcNow;

            // Act
            transaction.Update(newCategoryId, newPaymentMethod, newTransactionType, newAmount, newDescription, newDate, true);

            // Assert
            transaction.CategoryId.Should().Be(newCategoryId);
            transaction.PaymentMethod.Should().Be(newPaymentMethod);
            transaction.TransactionType.Should().Be(newTransactionType);
            transaction.Amount.Should().Be(newAmount);
            transaction.Description.Should().Be(newDescription);
            transaction.IsRecurring.Should().BeTrue();
            transaction.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Update_WithNullAmount_ShouldThrowDomainException()
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();

            // Act
            var act = () => transaction.Update(
                1, PaymentMethod.Cash, TransactionType.Income, null!, "Desc", DateTime.UtcNow);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(TransactionErrors.InvalidAmount.Description);
        }

        [Fact]
        public void Update_WithNullDescription_ShouldThrowDomainException()
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();

            // Act
            var act = () => transaction.Update(
                1, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), null!, DateTime.UtcNow);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(TransactionErrors.InvalidDescription.Description);
        }

        [Fact]
        public void Update_WithEmptyDescription_ShouldThrowDomainException()
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();

            // Act
            var act = () => transaction.Update(
                1, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), string.Empty, DateTime.UtcNow);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(TransactionErrors.InvalidDescription.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Update_WithInvalidCategoryId_ShouldThrowDomainException(int invalidCategoryId)
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();

            // Act
            var act = () => transaction.Update(
                invalidCategoryId, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), "Desc", DateTime.UtcNow);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(TransactionErrors.InvalidCategoryId.Description);
        }

        [Fact]
        public void Update_WithNullCategoryId_ShouldSucceed()
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();

            // Act
            transaction.Update(null, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), "Desc", DateTime.UtcNow);

            // Assert
            transaction.CategoryId.Should().BeNull();
        }

        [Fact]
        public void Update_ShouldConvertTransactionDateToUtc()
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();
            var localTime = DateTime.Now;

            // Act
            transaction.Update(1, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), "Desc", localTime);

            // Assert
            transaction.TransactionDate.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void Update_ShouldMarkAsUpdated()
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();
            var createdAt = transaction.CreatedAt;

            // Act
            transaction.Update(1, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), "Desc", DateTime.UtcNow);

            // Assert
            transaction.UpdatedAt.Should().NotBeNull();
            transaction.UpdatedAt.Should().BeOnOrAfter(createdAt);
        }

        [Theory]
        [InlineData(PaymentMethod.Cash)]
        [InlineData(PaymentMethod.CreditCard)]
        [InlineData(PaymentMethod.DebitCard)]
        [InlineData(PaymentMethod.BankTransfer)]
        [InlineData(PaymentMethod.Pix)]
        [InlineData(PaymentMethod.Other)]
        public void Update_WithAllPaymentMethods_ShouldSucceed(PaymentMethod method)
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();

            // Act
            transaction.Update(1, method, TransactionType.Income,
                Money.Create(100, Currency.BRL), "Desc", DateTime.UtcNow);

            // Assert
            transaction.PaymentMethod.Should().Be(method);
        }

        [Theory]
        [InlineData(TransactionType.Income)]
        [InlineData(TransactionType.Expense)]
        public void Update_WithAllTransactionTypes_ShouldSucceed(TransactionType type)
        {
            // Arrange
            var transaction = TransactionBuilder.Default().Build();

            // Act
            transaction.Update(1, PaymentMethod.Cash, type,
                Money.Create(100, Currency.BRL), "Desc", DateTime.UtcNow);

            // Assert
            transaction.TransactionType.Should().Be(type);
        }

        #endregion
    }
}
