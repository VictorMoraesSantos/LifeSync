using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Filters;
using TaskManager.Infrastructure.Persistence.Repositories;
using TaskManager.IntegrationTests.Fixtures;
using TaskManager.IntegrationTests.Helpers;

namespace TaskManager.IntegrationTests.Repositories
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class TaskLabelRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly TaskLabelRepository _repository;

        public TaskLabelRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TaskLabelRepository(_fixture.DbContext);
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region Create

        [Fact]
        public async Task Create_WithValidEntity_SavesToDatabase()
        {
            // Arrange
            var label = new TaskLabel("Important", LabelColor.Red, 1);

            // Act
            await _repository.Create(label);

            // Assert
            var saved = await _repository.GetById(label.Id);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Important");
            saved.LabelColor.Should().Be(LabelColor.Red);
            saved.UserId.Should().Be(1);
        }

        [Fact]
        public async Task Create_ShouldGenerateIdAutomatically()
        {
            // Arrange
            var label = new TaskLabel("Auto ID Label", LabelColor.Blue, 1);

            // Act
            await _repository.Create(label);

            // Assert
            label.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Create_ShouldTrimName()
        {
            // Arrange
            var label = new TaskLabel("  Trimmed Label  ", LabelColor.Green, 1);

            // Act
            await _repository.Create(label);

            // Assert
            var saved = await _repository.GetById(label.Id);
            saved!.Name.Should().Be("Trimmed Label");
        }

        [Fact]
        public async Task CreateRange_WithMultipleEntities_SavesAllToDatabase()
        {
            // Arrange
            var labels = new List<TaskLabel>
            {
                new("Label 1", LabelColor.Red, 1),
                new("Label 2", LabelColor.Blue, 1),
                new("Label 3", LabelColor.Green, 1)
            };

            // Act
            await _repository.CreateRange(labels);

            // Assert
            var all = await _repository.GetAll();
            all.Should().HaveCount(3);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GetById_WithExistingId_ReturnsEntity()
        {
            // Arrange
            var label = new TaskLabel("Find Me", LabelColor.Purple, 1);
            await _repository.Create(label);

            // Act
            var result = await _repository.GetById(label.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Find Me");
            result.LabelColor.Should().Be(LabelColor.Purple);
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetById(99999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task GetAll_WithNoLabels_ReturnsEmptyCollection()
        {
            // Act
            var result = await _repository.GetAll();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_WithMultipleLabels_ReturnsAllLabels()
        {
            // Arrange
            var labels = TestDataFactory.CreateTaskLabels(4, userId: 1);
            await _repository.CreateRange(labels);

            // Act
            var result = await _repository.GetAll();

            // Assert
            result.Should().HaveCount(4);
        }

        #endregion

        #region Find

        [Fact]
        public async Task Find_WithMatchingPredicate_ReturnsFilteredLabels()
        {
            // Arrange
            var label1 = new TaskLabel("User1 Label", LabelColor.Red, 1);
            var label2 = new TaskLabel("User2 Label", LabelColor.Blue, 2);
            var label3 = new TaskLabel("User1 Other", LabelColor.Green, 1);
            await _repository.CreateRange(new[] { label1, label2, label3 });

            // Act
            var result = await _repository.Find(l => l.UserId == 1);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Find_WithNoMatchingItems_ReturnsEmptyCollection()
        {
            // Arrange
            var label = new TaskLabel("Label", LabelColor.Red, 1);
            await _repository.Create(label);

            // Act
            var result = await _repository.Find(l => l.UserId == 999);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region FindByFilter

        [Fact]
        public async Task FindByFilter_WithUserIdFilter_ReturnsOnlyUserLabels()
        {
            // Arrange
            var labels1 = Enumerable.Range(1, 3).Select(i => new TaskLabel($"User1 Label {i}", LabelColor.Red, 1)).ToList();
            var labels2 = Enumerable.Range(1, 2).Select(i => new TaskLabel($"User2 Label {i}", LabelColor.Blue, 2)).ToList();
            await _repository.CreateRange(labels1.Concat(labels2).ToList());

            var filter = new TaskLabelQueryFilter(userId: 1);

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(3);
            total.Should().Be(3);
        }

        [Fact]
        public async Task FindByFilter_WithColorFilter_ReturnsOnlyMatchingColor()
        {
            // Arrange
            var label1 = new TaskLabel("Red Label", LabelColor.Red, 1);
            var label2 = new TaskLabel("Blue Label", LabelColor.Blue, 1);
            var label3 = new TaskLabel("Another Red", LabelColor.Red, 1);
            await _repository.CreateRange(new[] { label1, label2, label3 });

            var filter = new TaskLabelQueryFilter(labelColor: LabelColor.Red);

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(2);
            total.Should().Be(2);
        }

        [Fact]
        public async Task FindByFilter_WithNameContains_ReturnsMatchingLabels()
        {
            // Arrange
            var label1 = new TaskLabel("Work Project", LabelColor.Red, 1);
            var label2 = new TaskLabel("Personal", LabelColor.Blue, 1);
            var label3 = new TaskLabel("Work Home", LabelColor.Green, 1);
            await _repository.CreateRange(new[] { label1, label2, label3 });

            var filter = new TaskLabelQueryFilter(nameContains: "Work");

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(2);
            total.Should().Be(2);
        }

        [Fact]
        public async Task FindByFilter_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var labels = Enumerable.Range(1, 12)
                .Select(i => new TaskLabel($"Label {i:D2}", LabelColor.Blue, 1))
                .ToList();
            await _repository.CreateRange(labels);

            var filter = new TaskLabelQueryFilter(userId: 1, page: 2, pageSize: 5);

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(5);
            total.Should().Be(12);
        }

        [Fact]
        public async Task FindByFilter_WithNoMatchingFilters_ReturnsEmptyCollection()
        {
            // Arrange
            var label = new TaskLabel("Label", LabelColor.Red, 1);
            await _repository.Create(label);

            var filter = new TaskLabelQueryFilter(userId: 999);

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().BeEmpty();
            total.Should().Be(0);
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_WithValidEntity_PersistsChanges()
        {
            // Arrange
            var label = new TaskLabel("Original Name", LabelColor.Red, 1);
            await _repository.Create(label);

            // Act - TaskLabelRepository.GetById uses AsNoTracking, so we need to attach
            var entity = await _repository.GetById(label.Id);
            entity!.Update("Updated Name", LabelColor.Blue);
            await _repository.Update(entity);

            // Assert
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskLabelRepository(newContext);
            var updated = await repo.GetById(label.Id);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Updated Name");
            updated.LabelColor.Should().Be(LabelColor.Blue);
            updated.UpdatedAt.Should().NotBeNull();
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_WithExistingEntity_RemovesFromDatabase()
        {
            // Arrange
            var label = new TaskLabel("Delete Me", LabelColor.Red, 1);
            await _repository.Create(label);
            var id = label.Id;

            // Act
            var entity = await _repository.GetById(id);
            await _repository.Delete(entity!);

            // Assert
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskLabelRepository(newContext);
            var deleted = await repo.GetById(id);
            deleted.Should().BeNull();
        }

        #endregion

        #region All LabelColor Values

        [Theory]
        [InlineData(LabelColor.Red)]
        [InlineData(LabelColor.Green)]
        [InlineData(LabelColor.Blue)]
        [InlineData(LabelColor.Yellow)]
        [InlineData(LabelColor.Purple)]
        [InlineData(LabelColor.Orange)]
        [InlineData(LabelColor.Pink)]
        [InlineData(LabelColor.Brown)]
        [InlineData(LabelColor.Gray)]
        public async Task Create_WithEachLabelColor_PersistsCorrectly(LabelColor color)
        {
            // Arrange
            var label = new TaskLabel($"{color} Label", color, 1);

            // Act
            await _repository.Create(label);

            // Assert
            var saved = await _repository.GetById(label.Id);
            saved.Should().NotBeNull();
            saved!.LabelColor.Should().Be(color);
        }

        #endregion
    }
}
