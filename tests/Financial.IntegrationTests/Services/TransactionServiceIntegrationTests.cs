using Financial.Application.DTOs.Transaction;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Errors;
using Financial.Infrastructure.Persistence.Repositories;
using Financial.Infrastructure.Services;
using Financial.IntegrationTests.Fixtures;
using Financial.IntegrationTests.Helpers;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Financial.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class TransactionServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;

        public TransactionServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private TransactionService CreateService()
        {
            var context = _fixture.CreateNewContext();
            var repo = new TransactionRepository(context);
            return new TransactionService(repo, NullLogger<TransactionService>.Instance);
        }

        private async Task<Category> CreateCategoryAsync(int userId = 1)
        {
            var context = _fixture.CreateNewContext();
            var category = TestDataFactory.CreateCategory(userId);
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return category;
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateTransaction()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var service = CreateService();
            var dto = new CreateTransactionDTO(
                1, category.Id, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Test Transaction", DateTime.UtcNow);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateAsync_WithNullDTO_ShouldFail()
        {
            // Act
            var service = CreateService();
            var result = await service.CreateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(TransactionErrors.CreateError.Description);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingTransaction_ShouldReturnDTO()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateTransactionDTO(1, category.Id, PaymentMethod.Cash, TransactionType.Income,
                    Money.Create(500, Currency.USD), "Lookup Test", DateTime.UtcNow));

            // Act
            var service2 = CreateService();
            var result = await service2.GetByIdAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Description.Should().Be("Lookup Test");
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Act
            var service = CreateService();
            var result = await service.GetByIdAsync(99999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateTransaction()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateTransactionDTO(1, category.Id, PaymentMethod.Cash, TransactionType.Expense,
                    Money.Create(1000, Currency.BRL), "Old Description", DateTime.UtcNow));

            var updateDto = new UpdateTransactionDTO(
                createResult.Value, category.Id, PaymentMethod.DebitCard, TransactionType.Income,
                Money.Create(2000, Currency.USD), "Updated Description", DateTime.UtcNow);

            // Act
            var service2 = CreateService();
            var result = await service2.UpdateAsync(updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var service3 = CreateService();
            var updated = await service3.GetByIdAsync(createResult.Value);
            updated.Value.Description.Should().Be("Updated Description");
            updated.Value.PaymentMethod.Should().Be(PaymentMethod.DebitCard);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingTransaction_ShouldSucceed()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateTransactionDTO(1, category.Id, PaymentMethod.Pix, TransactionType.Expense,
                    Money.Create(1000, Currency.BRL), "To Delete", DateTime.UtcNow));

            // Act
            var service2 = CreateService();
            var result = await service2.DeleteAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldFail()
        {
            // Act
            var service = CreateService();
            var result = await service.DeleteAsync(99999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetByFilterAsync Tests

        [Fact]
        public async Task GetByFilterAsync_WithPaymentMethodFilter_ShouldReturnFilteredResults()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var service = CreateService();
            await service.CreateAsync(
                new CreateTransactionDTO(1, category.Id, PaymentMethod.Pix, TransactionType.Expense,
                    Money.Create(1000, Currency.BRL), "Pix Transaction", DateTime.UtcNow));

            var filter = new TransactionFilterDTO(
                null, 1, null, PaymentMethod.Pix, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, 1, 10);

            // Act
            var service2 = CreateService();
            var result = await service2.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().NotBeEmpty();
            result.Value.Pagination.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByFilterAsync_WithPagination_ShouldReturnPaginatedResults()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var service = CreateService();
            for (int i = 0; i < 3; i++)
            {
                await service.CreateAsync(
                    new CreateTransactionDTO(1, category.Id, PaymentMethod.Pix, TransactionType.Expense,
                        Money.Create(1000 + i, Currency.BRL), $"Transaction {i}", DateTime.UtcNow));
            }

            var filter = new TransactionFilterDTO(
                null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, 1, 2);

            // Act
            var service2 = CreateService();
            var result = await service2.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Count().Should().BeLessThanOrEqualTo(2);
        }

        #endregion
    }
}
