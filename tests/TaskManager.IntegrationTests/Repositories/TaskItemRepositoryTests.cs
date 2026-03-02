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
    public class TaskItemRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly TaskItemRepository _repository;

        public TaskItemRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TaskItemRepository(_fixture.DbContext);
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
            var taskItem = new TaskItem(
                "Integration Test Task",
                "Test Description",
                Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                1,
                null);

            // Act
            await _repository.Create(taskItem);

            // Assert
            var saved = await _repository.GetById(taskItem.Id);
            saved.Should().NotBeNull();
            saved!.Title.Should().Be("Integration Test Task");
            saved.Description.Should().Be("Test Description");
            saved.Priority.Should().Be(Priority.High);
            saved.UserId.Should().Be(1);
            saved.Status.Should().Be(Status.Pending);
        }

        [Fact]
        public async Task Create_ShouldGenerateIdAutomatically()
        {
            // Arrange
            var taskItem = new TaskItem(
                "Auto ID Task",
                "Description",
                Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                1,
                null);

            // Act
            await _repository.Create(taskItem);

            // Assert
            taskItem.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Create_ShouldSetCreatedAtAutomatically()
        {
            // Arrange
            var taskItem = new TaskItem(
                "CreatedAt Task",
                "Description",
                Priority.Low,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                1,
                null);

            // Act
            await _repository.Create(taskItem);

            // Assert
            var saved = await _repository.GetById(taskItem.Id);
            saved!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task CreateRange_WithMultipleEntities_SavesAllToDatabase()
        {
            // Arrange
            var items = Enumerable.Range(1, 5)
                .Select(i => new TaskItem(
                    $"Batch Task {i}",
                    $"Description {i}",
                    Priority.Medium,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                    1,
                    null))
                .ToList();

            // Act
            await _repository.CreateRange(items);

            // Assert
            var all = await _repository.GetAll();
            all.Should().HaveCount(5);
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GetById_WithExistingId_ReturnsEntity()
        {
            // Arrange
            var taskItem = new TaskItem(
                "Find Me",
                "Description",
                Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                1,
                null);
            await _repository.Create(taskItem);

            // Act
            var result = await _repository.GetById(taskItem.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("Find Me");
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetById(99999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_ShouldIncludeLabels()
        {
            // Arrange
            var label = new TaskLabel("Important", LabelColor.Red, 1);
            _fixture.DbContext.TaskLabels.Add(label);
            await _fixture.DbContext.SaveChangesAsync();

            var taskItem = new TaskItem(
                "Task With Label",
                "Description",
                Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                1,
                new List<TaskLabel> { label });
            await _repository.Create(taskItem);

            // Act - use a new context to ensure we're loading from DB
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskItemRepository(newContext);
            var result = await repo.GetById(taskItem.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Labels.Should().HaveCount(1);
            result.Labels.First().Name.Should().Be("Important");
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task GetAll_WithNoItems_ReturnsEmptyCollection()
        {
            // Act
            var result = await _repository.GetAll();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_WithMultipleItems_ReturnsAllItems()
        {
            // Arrange
            var items = TestDataFactory.CreateTaskItems(3, userId: 1);
            await _repository.CreateRange(items);

            // Act
            var result = await _repository.GetAll();

            // Assert
            result.Should().HaveCount(3);
        }

        #endregion

        #region Find

        [Fact]
        public async Task Find_WithMatchingPredicate_ReturnsFilteredItems()
        {
            // Arrange
            var item1 = new TaskItem("Task Alpha", "Desc", Priority.High, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var item2 = new TaskItem("Task Beta", "Desc", Priority.Low, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var item3 = new TaskItem("Task Gamma", "Desc", Priority.High, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 2, null);
            await _repository.CreateRange(new[] { item1, item2, item3 });

            // Act
            var result = await _repository.Find(t => t.UserId == 1);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Find_WithNoMatchingItems_ReturnsEmptyCollection()
        {
            // Arrange
            var item = new TaskItem("Task", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _repository.Create(item);

            // Act
            var result = await _repository.Find(t => t.UserId == 999);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region FindByFilter

        [Fact]
        public async Task FindByFilter_WithUserIdFilter_ReturnsOnlyUserItems()
        {
            // Arrange
            var items1 = Enumerable.Range(1, 3).Select(i =>
                new TaskItem($"User1 Task {i}", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 1, null)).ToList();
            var items2 = Enumerable.Range(1, 2).Select(i =>
                new TaskItem($"User2 Task {i}", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 2, null)).ToList();
            await _repository.CreateRange(items1.Concat(items2).ToList());

            var filter = new TaskItemQueryFilter(userId: 1);

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(3);
            total.Should().Be(3);
            results.Should().AllSatisfy(t => t.UserId.Should().Be(1));
        }

        [Fact]
        public async Task FindByFilter_WithStatusFilter_ReturnsOnlyMatchingStatus()
        {
            // Arrange
            var item1 = new TaskItem("Pending Task", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var item2 = new TaskItem("Completed Task", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            item2.ChangeStatus(Status.Completed);
            await _repository.CreateRange(new[] { item1, item2 });

            var filter = new TaskItemQueryFilter(status: Status.Pending);

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(1);
            results.First().Title.Should().Be("Pending Task");
        }

        [Fact]
        public async Task FindByFilter_WithPriorityFilter_ReturnsOnlyMatchingPriority()
        {
            // Arrange
            var item1 = new TaskItem("Urgent Task", "Desc", Priority.Urgent, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var item2 = new TaskItem("Low Task", "Desc", Priority.Low, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _repository.CreateRange(new[] { item1, item2 });

            var filter = new TaskItemQueryFilter(priority: Priority.Urgent);

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(1);
            results.First().Title.Should().Be("Urgent Task");
        }

        [Fact]
        public async Task FindByFilter_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var items = Enumerable.Range(1, 15)
                .Select(i => new TaskItem($"Task {i:D2}", "Description", Priority.Medium,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 1, null))
                .ToList();
            await _repository.CreateRange(items);

            var filter = new TaskItemQueryFilter(userId: 1, page: 2, pageSize: 5);

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(5);
            total.Should().Be(15);
        }

        [Fact]
        public async Task FindByFilter_WithTitleContains_ReturnsMatchingItems()
        {
            // Arrange
            var item1 = new TaskItem("Buy groceries", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var item2 = new TaskItem("Clean house", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var item3 = new TaskItem("Buy new shoes", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _repository.CreateRange(new[] { item1, item2, item3 });

            var filter = new TaskItemQueryFilter(titleContains: "Buy");

            // Act
            var (results, total) = await _repository.FindByFilter(filter);

            // Assert
            results.Should().HaveCount(2);
            total.Should().Be(2);
        }

        [Fact]
        public async Task FindByFilter_WithNoMatchingFilters_ReturnsEmptyCollection()
        {
            // Arrange
            var item = new TaskItem("Task", "Desc", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _repository.Create(item);

            var filter = new TaskItemQueryFilter(userId: 999);

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
            var taskItem = new TaskItem("Original Title", "Original Desc", Priority.Low,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _repository.Create(taskItem);

            // Act
            var entity = await _repository.GetById(taskItem.Id);
            entity!.Update("Updated Title", "Updated Desc", Status.InProgress, Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));
            await _repository.Update(entity);

            // Assert
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskItemRepository(newContext);
            var updated = await repo.GetById(taskItem.Id);
            updated.Should().NotBeNull();
            updated!.Title.Should().Be("Updated Title");
            updated.Description.Should().Be("Updated Desc");
            updated.Status.Should().Be(Status.InProgress);
            updated.Priority.Should().Be(Priority.High);
            updated.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Update_ChangeStatus_PersistsStatusChange()
        {
            // Arrange
            var taskItem = new TaskItem("Status Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _repository.Create(taskItem);

            // Act
            var entity = await _repository.GetById(taskItem.Id);
            entity!.ChangeStatus(Status.Completed);
            await _repository.Update(entity);

            // Assert
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskItemRepository(newContext);
            var updated = await repo.GetById(taskItem.Id);
            updated!.Status.Should().Be(Status.Completed);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_WithExistingEntity_RemovesFromDatabase()
        {
            // Arrange
            var taskItem = new TaskItem("Delete Me", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _repository.Create(taskItem);
            var id = taskItem.Id;

            // Act
            var entity = await _repository.GetById(id);
            await _repository.Delete(entity!);

            // Assert
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskItemRepository(newContext);
            var deleted = await repo.GetById(id);
            deleted.Should().BeNull();
        }

        #endregion

        #region Labels Relationship

        [Fact]
        public async Task Create_WithLabels_PersistsManyToManyRelationship()
        {
            // Arrange
            var label1 = new TaskLabel("Urgent", LabelColor.Red, 1);
            var label2 = new TaskLabel("Work", LabelColor.Blue, 1);
            _fixture.DbContext.TaskLabels.AddRange(label1, label2);
            await _fixture.DbContext.SaveChangesAsync();

            var taskItem = new TaskItem(
                "Task With Labels",
                "Description",
                Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                1,
                new List<TaskLabel> { label1, label2 });

            // Act
            await _repository.Create(taskItem);

            // Assert
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskItemRepository(newContext);
            var saved = await repo.GetById(taskItem.Id);
            saved.Should().NotBeNull();
            saved!.Labels.Should().HaveCount(2);
        }

        [Fact]
        public async Task Update_AddLabel_PersistsNewLabelRelationship()
        {
            // Arrange
            var label = new TaskLabel("New Label", LabelColor.Green, 1);
            _fixture.DbContext.TaskLabels.Add(label);
            await _fixture.DbContext.SaveChangesAsync();

            var taskItem = new TaskItem("Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _repository.Create(taskItem);

            // Act
            var entity = await _repository.GetById(taskItem.Id);
            entity!.AddLabel(label);
            await _repository.Update(entity);

            // Assert
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskItemRepository(newContext);
            var updated = await repo.GetById(taskItem.Id);
            updated!.Labels.Should().HaveCount(1);
            updated.Labels.First().Name.Should().Be("New Label");
        }

        [Fact]
        public async Task Update_RemoveLabel_PersistsLabelRemoval()
        {
            // Arrange
            var label = new TaskLabel("Remove Me", LabelColor.Yellow, 1);
            _fixture.DbContext.TaskLabels.Add(label);
            await _fixture.DbContext.SaveChangesAsync();

            var taskItem = new TaskItem("Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, new List<TaskLabel> { label });
            await _repository.Create(taskItem);

            // Act
            var entity = await _repository.GetById(taskItem.Id);
            entity!.RemoveLabel(label);
            await _repository.Update(entity);

            // Assert
            using var newContext = _fixture.CreateNewContext();
            var repo = new TaskItemRepository(newContext);
            var updated = await repo.GetById(taskItem.Id);
            updated!.Labels.Should().BeEmpty();
        }

        #endregion
    }
}
