using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Errors;
using Financial.Infrastructure.Persistence.Repositories;
using Financial.Infrastructure.Services;
using Financial.IntegrationTests.Fixtures;
using Financial.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Financial.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class RecurrenceScheduleServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;

        public RecurrenceScheduleServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private RecurrenceScheduleService CreateService()
        {
            var context = _fixture.CreateNewContext();
            var scheduleRepo = new RecurrenceScheduleRepository(context);
            var transactionRepo = new TransactionRepository(context);
            return new RecurrenceScheduleService(
                scheduleRepo,
                transactionRepo,
                context,
                NullLogger<RecurrenceScheduleService>.Instance);
        }

        private async Task<Transaction> CreateRecurringTransactionAsync(int userId = 1)
        {
            var context = _fixture.CreateNewContext();
            var category = TestDataFactory.CreateCategory(userId);
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            var transaction = TestDataFactory.CreateTransaction(userId, category.Id, true);
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            return transaction;
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateSchedule()
        {
            // Arrange
            var transaction = await CreateRecurringTransactionAsync();
            var service = CreateService();
            var dto = new CreateRecurrenceScheduleDTO(
                transaction.Id, RecurrenceFrequency.Monthly, DateTime.UtcNow);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateAsync_WithNonRecurringTransaction_ShouldFail()
        {
            // Arrange
            var context = _fixture.CreateNewContext();
            var category = TestDataFactory.CreateCategory();
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            var transaction = TestDataFactory.CreateTransaction(1, category.Id, false);
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            var service = CreateService();
            var dto = new CreateRecurrenceScheduleDTO(
                transaction.Id, RecurrenceFrequency.Monthly, DateTime.UtcNow);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.TransactionNotRecurring.Description);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateSchedule_ShouldFail()
        {
            // Arrange
            var transaction = await CreateRecurringTransactionAsync();
            var service = CreateService();
            var dto = new CreateRecurrenceScheduleDTO(
                transaction.Id, RecurrenceFrequency.Monthly, DateTime.UtcNow);

            await service.CreateAsync(dto);

            // Act - try to create a second schedule for the same transaction
            var service2 = CreateService();
            var result = await service2.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(RecurrenceScheduleErrors.ScheduleAlreadyExists.Description);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingSchedule_ShouldReturnDTO()
        {
            // Arrange
            var transaction = await CreateRecurringTransactionAsync();
            var service = CreateService();
            var endDate = DateTime.UtcNow.AddYears(1);
            var createResult = await service.CreateAsync(
                new CreateRecurrenceScheduleDTO(transaction.Id, RecurrenceFrequency.Weekly, DateTime.UtcNow, endDate));

            // Act
            var service2 = CreateService();
            var result = await service2.GetByIdAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.TransactionId.Should().Be(transaction.Id);
            result.Value.Frequency.Should().Be(RecurrenceFrequency.Weekly);
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
        public async Task UpdateAsync_WithValidData_ShouldUpdateSchedule()
        {
            // Arrange
            var transaction = await CreateRecurringTransactionAsync();
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateRecurrenceScheduleDTO(transaction.Id, RecurrenceFrequency.Monthly, DateTime.UtcNow));

            var updateDto = new UpdateRecurrenceScheduleDTO(
                createResult.Value, RecurrenceFrequency.Daily, DateTime.UtcNow.AddYears(1), 24);

            // Act
            var service2 = CreateService();
            var result = await service2.UpdateAsync(updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var service3 = CreateService();
            var updated = await service3.GetByIdAsync(createResult.Value);
            updated.Value!.Frequency.Should().Be(RecurrenceFrequency.Daily);
            updated.Value.MaxOccurrences.Should().Be(24);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingSchedule_ShouldSucceed()
        {
            // Arrange
            var transaction = await CreateRecurringTransactionAsync();
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateRecurrenceScheduleDTO(transaction.Id, RecurrenceFrequency.Monthly, DateTime.UtcNow));

            // Act
            var service2 = CreateService();
            var result = await service2.DeleteAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        #endregion

        #region DeactiveScheduleAsync Tests

        [Fact]
        public async Task DeactiveScheduleAsync_WithExistingSchedule_ShouldDeactivate()
        {
            // Arrange
            var transaction = await CreateRecurringTransactionAsync();
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateRecurrenceScheduleDTO(transaction.Id, RecurrenceFrequency.Monthly, DateTime.UtcNow));

            // Activate first
            var service2 = CreateService();
            await service2.ActiveScheduleAsync(createResult.Value);

            // Act
            var service3 = CreateService();
            var result = await service3.DeactiveScheduleAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        #endregion

        #region GetByFilterAsync Tests

        [Fact]
        public async Task GetByFilterAsync_WithFrequencyFilter_ShouldReturnFilteredResults()
        {
            // Arrange
            var transaction = await CreateRecurringTransactionAsync();
            var service = CreateService();
            await service.CreateAsync(
                new CreateRecurrenceScheduleDTO(transaction.Id, RecurrenceFrequency.Daily, DateTime.UtcNow, DateTime.UtcNow.AddYears(1)));

            var filter = new RecurrenceScheduleFilterDTO(
                Frequency: RecurrenceFrequency.Daily,
                Page: 1,
                PageSize: 10);

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
            // Arrange - create multiple schedules
            for (int i = 0; i < 3; i++)
            {
                var transaction = await CreateRecurringTransactionAsync();
                var service = CreateService();
                await service.CreateAsync(
                    new CreateRecurrenceScheduleDTO(transaction.Id, RecurrenceFrequency.Monthly, DateTime.UtcNow, DateTime.UtcNow.AddYears(1)));
            }

            var filter = new RecurrenceScheduleFilterDTO(Page: 1, PageSize: 2);

            // Act
            var service2 = CreateService();
            var result = await service2.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Count().Should().BeLessThanOrEqualTo(2);
        }

        #endregion

        #region GetActiveByUserIdAsync Tests

        [Fact]
        public async Task GetActiveByUserIdAsync_ShouldReturnActiveSchedulesForUser()
        {
            // Arrange
            var transaction = await CreateRecurringTransactionAsync(userId: 1);
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateRecurrenceScheduleDTO(transaction.Id, RecurrenceFrequency.Monthly, DateTime.UtcNow, DateTime.UtcNow.AddYears(1)));

            // Activate
            var service2 = CreateService();
            await service2.ActiveScheduleAsync(createResult.Value);

            // Act
            var service3 = CreateService();
            var result = await service3.GetActiveByUserIdAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();
        }

        #endregion
    }
}
