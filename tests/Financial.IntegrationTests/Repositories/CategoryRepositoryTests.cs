using Financial.Domain.Entities;
using Financial.Domain.Filters;
using Financial.Infrastructure.Persistence.Repositories;
using Financial.IntegrationTests.Fixtures;
using Financial.IntegrationTests.Helpers;
using FluentAssertions;

namespace Financial.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class CategoryRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private CategoryRepository _repository;

        public CategoryRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new CategoryRepository(_fixture.DbContext);
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
            _repository = new CategoryRepository(_fixture.CreateNewContext());
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region Create Tests

        [Fact]
        public async Task Create_WithValidEntity_ShouldPersist()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var repo = new CategoryRepository(_fixture.CreateNewContext());

            // Act
            await repo.Create(category);

            // Assert
            var context = _fixture.CreateNewContext();
            var saved = await new CategoryRepository(context).GetById(category.Id);
            saved.Should().NotBeNull();
            saved!.UserId.Should().Be(1);
        }

        [Fact]
        public async Task CreateRange_WithMultipleEntities_ShouldPersistAll()
        {
            // Arrange
            var categories = Enumerable.Range(1, 3)
                .Select(_ => TestDataFactory.CreateCategory())
                .ToList();

            var repo = new CategoryRepository(_fixture.CreateNewContext());

            // Act
            await repo.CreateRange(categories);

            // Assert
            var verifyRepo = new CategoryRepository(_fixture.CreateNewContext());
            var all = await verifyRepo.GetAll();
            all.Should().HaveCount(3);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnEntity()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var repoCreate = new CategoryRepository(_fixture.CreateNewContext());
            await repoCreate.Create(category);

            // Act
            var repo = new CategoryRepository(_fixture.CreateNewContext());
            var result = await repo.GetById(category.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(category.Id);
            result.Name.Should().Be(category.Name);
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

        #region GetAllByUserId Tests

        [Fact]
        public async Task GetAllByUserId_WithExistingUser_ShouldReturnCategories()
        {
            // Arrange
            var context = _fixture.CreateNewContext();
            var cat1 = TestDataFactory.CreateCategory(userId: 1);
            var cat2 = TestDataFactory.CreateCategory(userId: 1);
            var cat3 = TestDataFactory.CreateCategory(userId: 2);
            await context.Categories.AddRangeAsync(cat1, cat2, cat3);
            await context.SaveChangesAsync();

            // Act
            var repo = new CategoryRepository(_fixture.CreateNewContext());
            var result = await repo.GetAllByUserId(1);

            // Assert
            result.Should().HaveCount(2);
        }

        #endregion

        #region GetByNameContains Tests

        [Fact]
        public async Task GetByNameContains_WithMatchingName_ShouldReturnCategories()
        {
            // Arrange
            var context = _fixture.CreateNewContext();
            var cat = new Category(1, "Alimentacao Test", "Desc");
            await context.Categories.AddAsync(cat);
            await context.SaveChangesAsync();

            // Act
            var repo = new CategoryRepository(_fixture.CreateNewContext());
            var result = await repo.GetByNameContains("Alimentacao");

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var repoCreate = new CategoryRepository(_fixture.CreateNewContext());
            await repoCreate.Create(category);

            // Act
            var repoUpdate = new CategoryRepository(_fixture.CreateNewContext());
            var toUpdate = await repoUpdate.GetById(category.Id);
            toUpdate!.Update("Updated Name", "Updated Desc");
            await repoUpdate.Update(toUpdate);

            // Assert
            var verifyRepo = new CategoryRepository(_fixture.CreateNewContext());
            var updated = await verifyRepo.GetById(category.Id);
            updated!.Name.Should().Be("Updated Name");
            updated.Description.Should().Be("Updated Desc");
            updated.UpdatedAt.Should().NotBeNull();
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var repoCreate = new CategoryRepository(_fixture.CreateNewContext());
            await repoCreate.Create(category);

            // Act
            var repoDelete = new CategoryRepository(_fixture.CreateNewContext());
            var toDelete = await repoDelete.GetById(category.Id);
            await repoDelete.Delete(toDelete!);

            // Assert
            var verifyRepo = new CategoryRepository(_fixture.CreateNewContext());
            var deleted = await verifyRepo.GetById(category.Id);
            deleted.Should().BeNull();
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnAllCategories()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory();
            var repoCreate = new CategoryRepository(_fixture.CreateNewContext());
            await repoCreate.Create(category);

            // Act
            var repo = new CategoryRepository(_fixture.CreateNewContext());
            var result = await repo.GetAll();

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion

        #region FindByFilter Tests

        [Fact]
        public async Task FindByFilter_WithUserIdFilter_ShouldFilterCorrectly()
        {
            // Arrange
            var context = _fixture.CreateNewContext();
            var cat = TestDataFactory.CreateCategory(userId: 1);
            await context.Categories.AddAsync(cat);
            await context.SaveChangesAsync();

            var filter = new CategoryQueryFilter(userId: 1, page: 1, pageSize: 10);

            // Act
            var repo = new CategoryRepository(_fixture.CreateNewContext());
            var (items, totalCount) = await repo.FindByFilter(filter, CancellationToken.None);

            // Assert
            items.Should().NotBeEmpty();
            totalCount.Should().BeGreaterThan(0);
        }

        #endregion
    }
}
