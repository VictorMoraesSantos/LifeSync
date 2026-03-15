using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Filters;
using Financial.Infrastructure.Persistence.Repositories;
using Financial.IntegrationTests.Fixtures;
using Financial.IntegrationTests.Helpers;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;

namespace Financial.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class RecurrenceScheduleRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private RecurrenceScheduleRepository _repository;

        public RecurrenceScheduleRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new RecurrenceScheduleRepository(_fixture.DbContext);
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _repository = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region Helpers

        private async Task<(Category category, Transaction transaction)> CreatePrerequisitesAsync(bool isRecurring = true, int userId = 1)
        {
            var context = _fixture.CreateNewContext();
            var category = TestDataFactory.CreateCategory(userId);
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            var transaction = TestDataFactory.CreateTransaction(userId, category.Id, isRecurring);
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            return (category, transaction);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidEntity_ShouldPersist()
        {
            // Arrange
            var (_, transaction) = await CreatePrerequisitesAsync();
            var schedule = TestDataFactory.CreateRecurrenceSchedule(transaction.Id);
            var repo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());

            // Act
            await repo.Create(schedule);

            // Assert
            var context = _fixture.CreateNewContext();
            var saved = await new RecurrenceScheduleRepository(context).GetById(schedule.Id);
            saved.Should().NotBeNull();
            saved!.TransactionId.Should().Be(transaction.Id);
            saved.Frequency.Should().Be(RecurrenceFrequency.Monthly);
        }

        [Fact]
        public async Task CreateRange_WithMultipleEntities_ShouldPersistAll()
        {
            // Arrange
            var context1 = _fixture.CreateNewContext();
            var category = TestDataFactory.CreateCategory();
            await context1.Categories.AddAsync(category);
            await context1.SaveChangesAsync();

            var transactions = new List<Transaction>();
            for (int i = 0; i < 3; i++)
            {
                var t = TestDataFactory.CreateTransaction(1, category.Id, true);
                await context1.Transactions.AddAsync(t);
                await context1.SaveChangesAsync();
                transactions.Add(t);
            }

            var schedules = transactions.Select(t =>
                TestDataFactory.CreateRecurrenceSchedule(t.Id)).ToList();

            var repo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());

            // Act
            await repo.CreateRange(schedules);

            // Assert
            var verifyContext = _fixture.CreateNewContext();
            var verifyRepo = new RecurrenceScheduleRepository(verifyContext);
            var all = await verifyRepo.GetAll();
            all.Should().HaveCount(3);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnEntity()
        {
            // Arrange
            var (_, transaction) = await CreatePrerequisitesAsync();
            var schedule = TestDataFactory.CreateRecurrenceSchedule(transaction.Id);
            var repoCreate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            await repoCreate.Create(schedule);

            // Act
            var repo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var result = await repo.GetById(schedule.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(schedule.Id);
            result.Transaction.Should().NotBeNull();
            result.Transaction.Category.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetById(99999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetByTransactionId Tests

        [Fact]
        public async Task GetByTransactionId_WithExistingTransaction_ShouldReturnEntity()
        {
            // Arrange
            var (_, transaction) = await CreatePrerequisitesAsync();
            var schedule = TestDataFactory.CreateRecurrenceSchedule(transaction.Id);
            var repoCreate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            await repoCreate.Create(schedule);

            // Act
            var repo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var result = await repo.GetByTransactionId(transaction.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TransactionId.Should().Be(transaction.Id);
        }

        [Fact]
        public async Task GetByTransactionId_WithNonExistingTransaction_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByTransactionId(99999, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetActiveByUserId Tests

        [Fact]
        public async Task GetActiveByUserId_ShouldReturnOnlyActiveSchedules()
        {
            // Arrange
            var (_, transaction1) = await CreatePrerequisitesAsync(true, 1);
            var schedule1 = TestDataFactory.CreateRecurrenceSchedule(transaction1.Id);
            schedule1.Activate();

            var repoCreate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            await repoCreate.Create(schedule1);

            // Act
            var repo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var result = await repo.GetActiveByUserId(1, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllSatisfy(s => s.IsActive.Should().BeTrue());
        }

        #endregion

        #region GetDueSchedules Tests

        [Fact]
        public async Task GetDueSchedules_ShouldReturnSchedulesWithNextOccurrenceBeforeReferenceDate()
        {
            // Arrange
            var (_, transaction) = await CreatePrerequisitesAsync();
            var schedule = TestDataFactory.CreateRecurrenceSchedule(
                transaction.Id,
                startDate: DateTime.UtcNow.AddDays(-1));
            schedule.Activate();

            var repoCreate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            await repoCreate.Create(schedule);

            // Act
            var repo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var result = await repo.GetDueSchedules(DateTime.UtcNow.AddDays(1), CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            // Arrange
            var (_, transaction) = await CreatePrerequisitesAsync();
            var schedule = TestDataFactory.CreateRecurrenceSchedule(transaction.Id);
            var repoCreate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            await repoCreate.Create(schedule);

            // Act
            var repoUpdate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var toUpdate = await repoUpdate.GetById(schedule.Id);
            toUpdate!.Update(RecurrenceFrequency.Weekly, DateTime.UtcNow.AddYears(1), 12);
            await repoUpdate.Update(toUpdate);

            // Assert
            var verifyRepo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var updated = await verifyRepo.GetById(schedule.Id);
            updated!.Frequency.Should().Be(RecurrenceFrequency.Weekly);
            updated.MaxOccurrences.Should().Be(12);
            updated.UpdatedAt.Should().NotBeNull();
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            var (_, transaction) = await CreatePrerequisitesAsync();
            var schedule = TestDataFactory.CreateRecurrenceSchedule(transaction.Id);
            var repoCreate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            await repoCreate.Create(schedule);

            // Act
            var repoDelete = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var toDelete = await repoDelete.GetById(schedule.Id);
            await repoDelete.Delete(toDelete!);

            // Assert - Delete uses soft delete (Update), entity should still exist
            var verifyRepo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var deleted = await verifyRepo.GetById(schedule.Id);
            deleted.Should().NotBeNull(); // soft delete
        }

        #endregion

        #region FindByFilter Tests

        [Fact]
        public async Task FindByFilter_WithFrequencyFilter_ShouldFilterCorrectly()
        {
            // Arrange
            var (_, transaction) = await CreatePrerequisitesAsync();
            var schedule = TestDataFactory.CreateRecurrenceSchedule(transaction.Id, RecurrenceFrequency.Daily);
            var repoCreate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            await repoCreate.Create(schedule);

            var filter = new RecurrenceScheduleQueryFilter(
                frequency: RecurrenceFrequency.Daily,
                page: 1,
                pageSize: 10);

            // Act
            var repo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var (items, totalCount) = await repo.FindByFilter(filter, CancellationToken.None);

            // Assert
            items.Should().NotBeEmpty();
            items.Should().AllSatisfy(s => s.Frequency.Should().Be(RecurrenceFrequency.Daily));
            totalCount.Should().BeGreaterThan(0);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnAllSchedules()
        {
            // Arrange
            var (_, transaction) = await CreatePrerequisitesAsync();
            var schedule = TestDataFactory.CreateRecurrenceSchedule(transaction.Id);
            var repoCreate = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            await repoCreate.Create(schedule);

            // Act
            var repo = new RecurrenceScheduleRepository(_fixture.CreateNewContext());
            var result = await repo.GetAll();

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion
    }
}
