using Financial.Application.DTOs.Transaction;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;

namespace Financial.UnitTests.Application.Mappers
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class TransactionMapperTests
    {
        [Fact]
        public void ToEntity_WithValidDTO_ShouldMapCorrectly()
        {
            // Arrange
            var dto = new CreateTransactionDTO(
                UserId: 1,
                CategoryId: 5,
                PaymentMethod: PaymentMethod.Pix,
                TransactionType: TransactionType.Expense,
                Amount: Money.Create(1000, Currency.BRL),
                Description: "Test transaction",
                TransactionDate: DateTime.UtcNow,
                IsRecurring: false);

            // Act
            var entity = TransactionMapper.ToEntity(dto);

            // Assert
            entity.Should().NotBeNull();
            entity.UserId.Should().Be(dto.UserId);
            entity.CategoryId.Should().Be(dto.CategoryId);
            entity.PaymentMethod.Should().Be(dto.PaymentMethod);
            entity.TransactionType.Should().Be(dto.TransactionType);
            entity.Amount.Should().Be(dto.Amount);
            entity.Description.Should().Be(dto.Description);
            entity.IsRecurring.Should().Be(dto.IsRecurring);
        }

        [Fact]
        public void ToEntity_WithNullCategoryId_ShouldMapCorrectly()
        {
            // Arrange
            var dto = new CreateTransactionDTO(
                UserId: 1,
                CategoryId: null,
                PaymentMethod: PaymentMethod.Cash,
                TransactionType: TransactionType.Income,
                Amount: Money.Create(500, Currency.USD),
                Description: "No category",
                TransactionDate: DateTime.UtcNow);

            // Act
            var entity = TransactionMapper.ToEntity(dto);

            // Assert
            entity.CategoryId.Should().BeNull();
        }

        [Fact]
        public void ToDTO_WithValidEntity_ShouldMapCorrectly()
        {
            // Arrange
            var entity = new Transaction(
                1, null, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow);

            // Act
            var dto = TransactionMapper.ToDTO(entity);

            // Assert
            dto.Should().NotBeNull();
            dto.UserId.Should().Be(entity.UserId);
            dto.PaymentMethod.Should().Be(entity.PaymentMethod);
            dto.TransactionType.Should().Be(entity.TransactionType);
            dto.Amount.Should().Be(entity.Amount);
            dto.Description.Should().Be(entity.Description);
            dto.IsRecurring.Should().Be(entity.IsRecurring);
            dto.Category.Should().BeNull();
        }

        [Fact]
        public void ToDTO_WithRecurringTransaction_ShouldMapIsRecurring()
        {
            // Arrange
            var entity = new Transaction(
                1, null, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(500, Currency.USD), "Recurring", DateTime.UtcNow, true);

            // Act
            var dto = TransactionMapper.ToDTO(entity);

            // Assert
            dto.IsRecurring.Should().BeTrue();
        }

        [Theory]
        [InlineData(PaymentMethod.Cash)]
        [InlineData(PaymentMethod.CreditCard)]
        [InlineData(PaymentMethod.DebitCard)]
        [InlineData(PaymentMethod.BankTransfer)]
        [InlineData(PaymentMethod.Pix)]
        [InlineData(PaymentMethod.Other)]
        public void ToEntity_WithAllPaymentMethods_ShouldMapCorrectly(PaymentMethod method)
        {
            // Arrange
            var dto = new CreateTransactionDTO(
                1, null, method, TransactionType.Expense,
                Money.Create(100, Currency.BRL), "Test", DateTime.UtcNow);

            // Act
            var entity = TransactionMapper.ToEntity(dto);

            // Assert
            entity.PaymentMethod.Should().Be(method);
        }
    }
}
