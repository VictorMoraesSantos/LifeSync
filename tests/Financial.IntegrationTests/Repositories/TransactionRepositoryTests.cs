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
    public class TransactionRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private TransactionRepository _repository;

        public TransactionRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TransactionRepository(_fixture.DbContext);
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _repository = new TransactionRepository(_fixture.CreateNewContext());
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region Helpers

        private async Task<Category> CreateCategoryAsync(int userId = 1)
        {
            var context = _fixture.CreateNewContext();
            var category = TestDataFactory.CreateCategory(userId);
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return category;
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidEntity_ShouldPersist()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var transaction = TestDataFactory.CreateTransaction(1, category.Id);
            var repo = new TransactionRepository(_fixture.CreateNewContext());

            // Act
            await repo.Create(transaction);

            // Assert
            var context = _fixture.CreateNewContext();
            var saved = await new TransactionRepository(context).GetById(transaction.Id);
            saved.Should().NotBeNull();
            saved!.UserId.Should().Be(1);
            saved.CategoryId.Should().Be(category.Id);
        }

        [Fact]
        public async Task CreateRange_WithMultipleEntities_ShouldPersistAll()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var transactions = Enumerable.Range(1, 3)
                .Select(_ => TestDataFactory.CreateTransaction(1, category.Id))
                .ToList();

            var repo = new TransactionRepository(_fixture.CreateNewContext());

            // Act
            await repo.CreateRange(transactions);

            // Assert
            var verifyRepo = new TransactionRepository(_fixture.CreateNewContext());
            var all = await verifyRepo.GetAll();
            all.Should().HaveCount(3);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnEntityWithCategory()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var transaction = TestDataFactory.CreateTransaction(1, category.Id);
            var repoCreate = new TransactionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(transaction);

            // Act
            var repo = new TransactionRepository(_fixture.CreateNewContext());
            var result = await repo.GetById(transaction.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(transaction.Id);
            result.Category.Should().NotBeNull();
            result.Category!.Id.Should().Be(category.Id);
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

        #region Update Tests

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var transaction = TestDataFactory.CreateTransaction(1, category.Id);
            var repoCreate = new TransactionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(transaction);

            // Act
            var repoUpdate = new TransactionRepository(_fixture.CreateNewContext());
            var toUpdate = await repoUpdate.GetById(transaction.Id);
            toUpdate!.Update(
                category.Id, PaymentMethod.DebitCard, TransactionType.Income,
                Money.Create(5000, Currency.USD), "Updated Description", DateTime.UtcNow);
            await repoUpdate.Update(toUpdate);

            // Assert
            var verifyRepo = new TransactionRepository(_fixture.CreateNewContext());
            var updated = await verifyRepo.GetById(transaction.Id);
            updated!.PaymentMethod.Should().Be(PaymentMethod.DebitCard);
            updated.TransactionType.Should().Be(TransactionType.Income);
            updated.Description.Should().Be("Updated Description");
            updated.UpdatedAt.Should().NotBeNull();
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var transaction = TestDataFactory.CreateTransaction(1, category.Id);
            var repoCreate = new TransactionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(transaction);

            // Act
            var repoDelete = new TransactionRepository(_fixture.CreateNewContext());
            var toDelete = await repoDelete.GetById(transaction.Id);
            await repoDelete.Delete(toDelete!);

            // Assert
            var verifyRepo = new TransactionRepository(_fixture.CreateNewContext());
            var deleted = await verifyRepo.GetById(transaction.Id);
            deleted.Should().BeNull();
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnAllTransactionsWithCategories()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var transaction = TestDataFactory.CreateTransaction(1, category.Id);
            var repoCreate = new TransactionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(transaction);

            // Act
            var repo = new TransactionRepository(_fixture.CreateNewContext());
            var result = await repo.GetAll();

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion

        #region Find Tests

        [Fact]
        public async Task Find_WithPredicate_ShouldReturnMatchingEntities()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var transaction = TestDataFactory.CreateTransaction(1, category.Id);
            var repoCreate = new TransactionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(transaction);

            // Act
            var repo = new TransactionRepository(_fixture.CreateNewContext());
            var result = await repo.Find(t => t.UserId == 1);

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion

        #region FindByFilter Tests

        [Fact]
        public async Task FindByFilter_WithUserIdFilter_ShouldFilterCorrectly()
        {
            // Arrange
            var category = await CreateCategoryAsync();
            var transaction = TestDataFactory.CreateTransaction(1, category.Id);
            var repoCreate = new TransactionRepository(_fixture.CreateNewContext());
            await repoCreate.Create(transaction);

            var filter = new TransactionQueryFilter(userId: 1, page: 1, pageSize: 10);

            // Act
            var repo = new TransactionRepository(_fixture.CreateNewContext());
            var (items, totalCount) = await repo.FindByFilter(filter, CancellationToken.None);

            // Assert
            items.Should().NotBeEmpty();
            totalCount.Should().BeGreaterThan(0);
        }

        #endregion
    }
}
