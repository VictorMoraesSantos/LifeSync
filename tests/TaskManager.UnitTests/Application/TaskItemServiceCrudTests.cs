using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.UnitTests.Application;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class TaskItemServiceCrudTests
{
    private readonly Mock<ITaskItemRepository> _mockRepository;
    private readonly Mock<ITaskLabelRepository> _mockLabelRepository;
    private readonly Mock<ILogger<TaskItemService>> _mockLogger;
    private readonly TaskItemService _service;

    public TaskItemServiceCrudTests()
    {
        _mockRepository = new Mock<ITaskItemRepository>();
        _mockLabelRepository = new Mock<ITaskLabelRepository>();
        _mockLogger = new Mock<ILogger<TaskItemService>>();
        _service = new TaskItemService(_mockRepository.Object, _mockLogger.Object, _mockLabelRepository.Object);
    }

    private static TaskItem CreateTaskItem(
        string title = "Valid Title",
        string description = "Valid Description",
        Priority priority = Priority.Medium,
        int userId = 1,
        int daysFromNow = 7)
        => new TaskItem(title, description, priority, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysFromNow)), userId, null);

    // ─── CreateAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldReturnSuccessWithId()
    {
        var dto = new CreateTaskItemDTO("Title", "Description", Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), 1, null);

        _mockRepository.Setup(r => r.Create(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.Create(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullDto_ShouldReturnFailure()
    {
        var result = await _service.CreateAsync(null!, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.Create(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithExistingLabels_ShouldAddLabelsToTaskItem()
    {
        var labelId = 10;
        var label = new TaskLabel("Work", LabelColor.Blue, 1);
        var dto = new CreateTaskItemDTO("Title", "Description", Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), 1, new List<int> { labelId });

        _mockLabelRepository.Setup(r => r.GetById(labelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);
        _mockRepository.Setup(r => r.Create(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _mockLabelRepository.Verify(r => r.GetById(labelId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenLabelNotFound_ShouldReturnFailure()
    {
        var dto = new CreateTaskItemDTO("Title", "Description", Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), 1, new List<int> { 999 });

        _mockLabelRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskLabel?)null);

        var result = await _service.CreateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.Create(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── UpdateAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenTaskExistsAndDtoIsValid_ShouldReturnSuccess()
    {
        var task = CreateTaskItem();
        var dto = new UpdateTaskItemDTO(1, "New Title", "New Description", Status.InProgress,
            Priority.High, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)));

        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _mockRepository.Setup(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _mockRepository.Verify(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskNotFound_ShouldReturnFailure()
    {
        var dto = new UpdateTaskItemDTO(999, "New Title", "New Description", Status.InProgress,
            Priority.High, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)));

        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var result = await _service.UpdateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Description.Should().Contain("999");
    }

    [Fact]
    public async Task UpdateAsync_WithNullDto_ShouldReturnFailure()
    {
        var result = await _service.UpdateAsync(null!, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── DeleteAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenTaskExists_ShouldReturnSuccess()
    {
        var task = CreateTaskItem();
        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _mockRepository.Setup(r => r.Delete(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.Delete(task, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskNotFound_ShouldReturnFailure()
    {
        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var result = await _service.DeleteAsync(999, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.Delete(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── AddLabelAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task AddLabelAsync_WhenTaskAndLabelExist_ShouldReturnSuccess()
    {
        var task = CreateTaskItem();
        var label = new TaskLabel("Work", LabelColor.Blue, 1);
        var dto = new UpdateLabelsDTO(1, new List<int> { 10 });

        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _mockLabelRepository.Setup(r => r.GetById(10, It.IsAny<CancellationToken>())).ReturnsAsync(label);
        _mockRepository.Setup(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.AddLabelAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddLabelAsync_WhenTaskNotFound_ShouldReturnFailure()
    {
        var dto = new UpdateLabelsDTO(999, new List<int> { 1 });
        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var result = await _service.AddLabelAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Description.Should().Contain("999");
    }

    [Fact]
    public async Task AddLabelAsync_WithNullDto_ShouldReturnFailure()
    {
        var result = await _service.AddLabelAsync(null!, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── RemoveLabelAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveLabelAsync_WhenTaskHasLabel_ShouldRemoveAndReturnSuccess()
    {
        var label = new TaskLabel("Work", LabelColor.Blue, 1);
        var task = CreateTaskItem();
        task.AddLabel(label);
        var dto = new UpdateLabelsDTO(1, new List<int> { 10 });

        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _mockLabelRepository.Setup(r => r.GetById(10, It.IsAny<CancellationToken>())).ReturnsAsync(label);
        _mockRepository.Setup(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.RemoveLabelAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveLabelAsync_WhenTaskNotFound_ShouldReturnFailure()
    {
        var dto = new UpdateLabelsDTO(999, new List<int> { 1 });
        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var result = await _service.RemoveLabelAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveLabelAsync_WithNullDto_ShouldReturnFailure()
    {
        var result = await _service.RemoveLabelAsync(null!, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    // ─── CreateRangeAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateRangeAsync_WithValidDtos_ShouldReturnSuccessWithIds()
    {
        var dtos = new List<CreateTaskItemDTO>
        {
            new("Task 1", "Desc 1", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null),
            new("Task 2", "Desc 2", Priority.High, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), 1, null)
        };

        _mockRepository.Setup(r => r.CreateRange(It.IsAny<IEnumerable<TaskItem>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateRangeAsync(dtos, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateRangeAsync_WithEmptyList_ShouldReturnFailure()
    {
        var result = await _service.CreateRangeAsync(new List<CreateTaskItemDTO>(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.CreateRange(It.IsAny<IEnumerable<TaskItem>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── DeleteRangeAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteRangeAsync_WhenAllTasksExist_ShouldReturnSuccess()
    {
        var task1 = CreateTaskItem("Task 1");
        var task2 = CreateTaskItem("Task 2");

        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(task1);
        _mockRepository.Setup(r => r.GetById(2, It.IsAny<CancellationToken>())).ReturnsAsync(task2);
        _mockRepository.Setup(r => r.Update(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.DeleteRangeAsync(new[] { 1, 2 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRangeAsync_WhenSomeTasksNotFound_ShouldReturnFailure()
    {
        var task = CreateTaskItem();
        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>())).ReturnsAsync((TaskItem?)null);

        var result = await _service.DeleteRangeAsync(new[] { 1, 999 }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteRangeAsync_WithEmptyIds_ShouldReturnFailure()
    {
        var result = await _service.DeleteRangeAsync(new List<int>(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    // ─── GetAllAsync (with items) ─────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenRepositoryHasItems_ShouldReturnMappedDtos()
    {
        var tasks = Enumerable.Range(1, 3).Select(i => CreateTaskItem($"Task {i}")).ToList();
        _mockRepository.Setup(r => r.GetAll(It.IsAny<CancellationToken>())).ReturnsAsync(tasks);

        var result = await _service.GetAllAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }
}
