using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Filters;
using TaskManager.Infrastructure.Persistence.Repositories;
using TaskManager.IntegrationTests.Fixtures;

namespace TaskManager.IntegrationTests.Repositories;

[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class TaskItemRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly TaskItemRepository _repository;

    public TaskItemRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new TaskItemRepository(_fixture.DbContext);
    }

    private static TaskItem CreateTaskItem(
        string title = "Integration Test Task",
        string description = "Integration Test Description",
        Priority priority = Priority.Medium,
        int userId = 1,
        int daysFromNow = 7)
        => new TaskItem(title, description, priority, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysFromNow)), userId, null);

    // ─── GetById ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_WithExistingTaskItem_ReturnsTaskItem()
    {
        await _fixture.ResetDatabaseAsync();

        var task = CreateTaskItem("Fetch Me", priority: Priority.High);
        await _repository.Create(task);

        var result = await _repository.GetById(task.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Fetch Me");
        result.Priority.Should().Be(Priority.High);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ReturnsNull()
    {
        await _fixture.ResetDatabaseAsync();

        var result = await _repository.GetById(99999, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetById_ShouldIncludeLabels()
    {
        await _fixture.ResetDatabaseAsync();

        var label = new TaskLabel("Work", LabelColor.Blue, 1);
        _fixture.DbContext.TaskLabels.Add(label);
        await _fixture.DbContext.SaveChangesAsync();

        var task = CreateTaskItem("Task With Labels");
        task.AddLabel(label);
        await _repository.Create(task);

        var result = await _repository.GetById(task.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Labels.Should().HaveCount(1);
        result.Labels.First().Name.Should().Be("Work");
    }

    // ─── GetAll ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_WhenDatabaseIsEmpty_ReturnsEmptyList()
    {
        await _fixture.ResetDatabaseAsync();

        var result = await _repository.GetAll(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_WhenDatabaseHasItems_ReturnsAllItems()
    {
        await _fixture.ResetDatabaseAsync();

        var tasks = new List<TaskItem>
        {
            CreateTaskItem("Task 1"),
            CreateTaskItem("Task 2"),
            CreateTaskItem("Task 3")
        };

        foreach (var task in tasks)
            await _repository.Create(task);

        var result = await _repository.GetAll(CancellationToken.None);

        result.Should().HaveCount(3);
        result.Select(t => t!.Title).Should().Contain(new[] { "Task 1", "Task 2", "Task 3" });
    }

    // ─── Create ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_WithValidTaskItem_SavesToDatabase()
    {
        await _fixture.ResetDatabaseAsync();

        var task = CreateTaskItem("New Task", priority: Priority.Urgent, userId: 5);

        await _repository.Create(task);

        var saved = await _repository.GetById(task.Id, CancellationToken.None);
        saved.Should().NotBeNull();
        saved!.Title.Should().Be("New Task");
        saved.Priority.Should().Be(Priority.Urgent);
        saved.UserId.Should().Be(5);
        saved.Status.Should().Be(Status.Pending);
    }

    // ─── Update ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_WithChangedProperties_PersistsChangesToDatabase()
    {
        await _fixture.ResetDatabaseAsync();

        var task = CreateTaskItem("Original Title");
        await _repository.Create(task);

        task.Update("Updated Title", "Updated Description", Status.InProgress, Priority.High,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));
        await _repository.Update(task);

        var updated = await _repository.GetById(task.Id, CancellationToken.None);
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("Updated Title");
        updated.Status.Should().Be(Status.InProgress);
        updated.Priority.Should().Be(Priority.High);
        updated.UpdatedAt.Should().NotBeNull();
    }

    // ─── Delete ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_ExistingTaskItem_RemovesFromQueryResults()
    {
        await _fixture.ResetDatabaseAsync();

        var task = CreateTaskItem("To Delete");
        await _repository.Create(task);
        var taskId = task.Id;

        await _repository.Delete(task);

        // After deletion, the item should not be accessible
        // (either hard-deleted or soft-deleted with a query filter on IsDeleted)
        var result = await _repository.GetById(taskId, CancellationToken.None);
        result.Should().BeNull();
    }

    // ─── CreateRange ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateRange_WithMultipleTasks_SavesAllToDatabase()
    {
        await _fixture.ResetDatabaseAsync();

        var tasks = new List<TaskItem>
        {
            CreateTaskItem("Batch Task 1"),
            CreateTaskItem("Batch Task 2"),
            CreateTaskItem("Batch Task 3")
        };

        await _repository.CreateRange(tasks);

        var all = await _repository.GetAll(CancellationToken.None);
        all.Should().HaveCount(3);
    }

    // ─── FindByFilter ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task FindByFilter_FilterByUserId_ReturnsMatchingItems()
    {
        await _fixture.ResetDatabaseAsync();

        var tasksUser1 = Enumerable.Range(1, 3).Select(_ => CreateTaskItem(userId: 1)).ToList();
        var tasksUser2 = Enumerable.Range(1, 2).Select(_ => CreateTaskItem(userId: 2)).ToList();

        foreach (var task in tasksUser1.Concat(tasksUser2))
            await _repository.Create(task);

        var filter = new TaskItemQueryFilter(null, 1, null, null, null, null, null, null, null, null, null, null, 1, 10);
        var (items, totalCount) = await _repository.FindByFilter(filter, CancellationToken.None);

        items.Should().HaveCount(3);
        totalCount.Should().Be(3);
        items.Should().AllSatisfy(t => t.UserId.Should().Be(1));
    }

    [Fact]
    public async Task FindByFilter_FilterByStatus_ReturnsMatchingItems()
    {
        await _fixture.ResetDatabaseAsync();

        var pendingTask = CreateTaskItem("Pending Task");
        var inProgressTask = CreateTaskItem("In Progress Task");
        inProgressTask.Update("In Progress Task", "Description", Status.InProgress, Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)));

        await _repository.Create(pendingTask);
        await _repository.Create(inProgressTask);

        var filter = new TaskItemQueryFilter(null, null, null, Status.Pending, null, null, null, null, null, null, null, null, 1, 10);
        var (items, totalCount) = await _repository.FindByFilter(filter, CancellationToken.None);

        items.Should().HaveCount(1);
        items.First().Title.Should().Be("Pending Task");
    }

    [Fact]
    public async Task FindByFilter_FilterByPriority_ReturnsMatchingItems()
    {
        await _fixture.ResetDatabaseAsync();

        var urgentTask = CreateTaskItem("Urgent Task", priority: Priority.Urgent);
        var lowTask = CreateTaskItem("Low Task", priority: Priority.Low);

        await _repository.Create(urgentTask);
        await _repository.Create(lowTask);

        var filter = new TaskItemQueryFilter(null, null, null, null, Priority.Urgent, null, null, null, null, null, null, null, 1, 10);
        var (items, _) = await _repository.FindByFilter(filter, CancellationToken.None);

        items.Should().HaveCount(1);
        items.First().Priority.Should().Be(Priority.Urgent);
    }

    [Fact]
    public async Task FindByFilter_FilterByTitleContains_ReturnsMatchingItems()
    {
        await _fixture.ResetDatabaseAsync();

        await _repository.Create(CreateTaskItem("Meeting with team"));
        await _repository.Create(CreateTaskItem("Shopping list"));
        await _repository.Create(CreateTaskItem("Team standup"));

        var filter = new TaskItemQueryFilter(null, null, "team", null, null, null, null, null, null, null, null, null, 1, 10);
        var (items, _) = await _repository.FindByFilter(filter, CancellationToken.None);

        items.Should().HaveCount(2);
        items.Should().AllSatisfy(t => t.Title.ToLower().Should().Contain("team"));
    }

    [Fact]
    public async Task FindByFilter_WithPagination_ReturnsCorrectPage()
    {
        await _fixture.ResetDatabaseAsync();

        var tasks = Enumerable.Range(1, 15).Select(i => CreateTaskItem($"Task {i:00}")).ToList();
        foreach (var task in tasks)
            await _repository.Create(task);

        var filter = new TaskItemQueryFilter(null, null, null, null, null, null, null, null, null, null, null, null, 2, 5);
        var (items, totalCount) = await _repository.FindByFilter(filter, CancellationToken.None);

        totalCount.Should().Be(15);
        items.Should().HaveCount(5);
    }
}
