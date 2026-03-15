using BuildingBlocks.Results;
using Financial.Application.DTOs.Transaction;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Errors;
using Financial.Domain.Filters;
using Financial.Domain.Repositories;
using Financial.Infrastructure.Services;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Financial.UnitTests.Application
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _repoMock;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _repoMock = new Mock<ITransactionRepository>();
            _service = new TransactionService(
                _repoMock.Object,
                NullLogger<TransactionService>.Instance);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDTO_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new CreateTransactionDTO(
                1, 5, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow);

            _repoMock
                .Setup(x => x.Create(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repoMock.Verify(x => x.Create(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNullDTO_ShouldReturnFailure()
        {
            // Act
            var result = await _service.CreateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(TransactionErrors.CreateError.Description);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetByIdAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(TransactionErrors.InvalidId.Description);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            _repoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnTransaction()
        {
            // Arrange
            var transaction = new Transaction(
                1, null, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow);

            _repoMock
                .Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithNullDTO_ShouldReturnFailure()
        {
            // Act
            var result = await _service.UpdateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Arrange
            var dto = new UpdateTransactionDTO(
                0, null, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), "Test", DateTime.UtcNow);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(TransactionErrors.InvalidId.Description);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            var dto = new UpdateTransactionDTO(
                999, null, PaymentMethod.Cash, TransactionType.Income,
                Money.Create(100, Currency.BRL), "Test", DateTime.UtcNow);

            _repoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction?)null);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithValidDTO_ShouldReturnSuccess()
        {
            // Arrange
            var transaction = new Transaction(
                1, 5, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Old", DateTime.UtcNow);

            var dto = new UpdateTransactionDTO(
                1, 10, PaymentMethod.DebitCard, TransactionType.Income,
                Money.Create(2000, Currency.USD), "Updated", DateTime.UtcNow);

            _repoMock
                .Setup(x => x.GetById(dto.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            _repoMock
                .Setup(x => x.Update(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
            _repoMock.Verify(x => x.Update(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.DeleteAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(TransactionErrors.InvalidId.Description);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            _repoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var transaction = new Transaction(
                1, null, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow);

            _repoMock
                .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            _repoMock
                .Setup(x => x.Delete(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repoMock.Verify(x => x.Delete(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetByUserIdAsync Tests

        [Fact]
        public async Task GetByUserIdAsync_WithInvalidUserId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetByUserIdAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task GetByUserIdAsync_WithValidUserId_ShouldReturnTransactions()
        {
            // Arrange
            var transactions = new List<Transaction?>
            {
                new Transaction(1, null, PaymentMethod.Pix, TransactionType.Expense,
                    Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow)
            };

            _repoMock
                .Setup(x => x.Find(It.IsAny<System.Linq.Expressions.Expression<Func<Transaction, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            // Act
            var result = await _service.GetByUserIdAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        #endregion

        #region GetByFilterAsync Tests

        [Fact]
        public async Task GetByFilterAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var filter = new TransactionFilterDTO(
                null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, 1, 10);

            _repoMock
                .Setup(x => x.FindByFilter(It.IsAny<TransactionQueryFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Enumerable.Empty<Transaction>(), 0));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByFilterAsync_WithResults_ShouldReturnPaginatedData()
        {
            // Arrange
            var filter = new TransactionFilterDTO(
                null, 1, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, 1, 10);

            var transactions = new List<Transaction>
            {
                new Transaction(1, null, PaymentMethod.Pix, TransactionType.Expense,
                    Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow)
            };

            _repoMock
                .Setup(x => x.FindByFilter(It.IsAny<TransactionQueryFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((transactions, 1));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.Pagination.Should().NotBeNull();
        }

        #endregion
    }
}
