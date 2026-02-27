using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Filters;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.UnitTests.Application;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class TaskLabelServiceTests
{
    private readonly Mock<ITaskLabelRepository> _mockRepository;
    private readonly Mock<ILogger<TaskLabelService>> _mockLogger;
    private readonly TaskLabelService _service;

    public TaskLabelServiceTests()
    {
        _mockRepository = new Mock<ITaskLabelRepository>();
        _mockLogger = new Mock<ILogger<TaskLabelService>>();
        _service = new TaskLabelService(_mockRepository.Object, _mockLogger.Object);
    }

    private static TaskLabel CreateLabel(string name = "Work", LabelColor color = LabelColor.Blue, int userId = 1)
        => new TaskLabel(name, color, userId);

    // ─── GetAllAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsEmptyList_ShouldReturnEmptySuccessResult()
    {
        _mockRepository.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskLabel>());

        var result = await _service.GetAllAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryReturnsItems_ShouldReturnMappedDtos()
    {
        var labels = new List<TaskLabel>
        {
            CreateLabel("Work", LabelColor.Blue),
            CreateLabel("Personal", LabelColor.Green),
            CreateLabel("Urgent", LabelColor.Red)
        };

        _mockRepository.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(labels);

        var result = await _service.GetAllAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.Select(l => l.Name).Should().Contain(new[] { "Work", "Personal", "Urgent" });
    }

    // ─── GetByIdAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenLabelExists_ShouldReturnSuccessWithDto()
    {
        var label = CreateLabel("Work", LabelColor.Blue);
        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);

        var result = await _service.GetByIdAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Work");
        result.Value.LabelColor.Should().Be(LabelColor.Blue);
    }

    [Fact]
    public async Task GetByIdAsync_WhenLabelDoesNotExist_ShouldReturnFailure()
    {
        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskLabel?)null);

        var result = await _service.GetByIdAsync(999, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Description.Should().Contain("999");
    }

    // ─── CreateAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldReturnSuccessWithId()
    {
        var dto = new CreateTaskLabelDTO("Work", LabelColor.Blue, 1);

        _mockRepository.Setup(r => r.Create(It.IsAny<TaskLabel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.Create(It.IsAny<TaskLabel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullDto_ShouldReturnFailure()
    {
        var result = await _service.CreateAsync(null!, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.Create(It.IsAny<TaskLabel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── UpdateAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenLabelExistsAndDtoIsValid_ShouldReturnSuccess()
    {
        var label = CreateLabel("Old Name", LabelColor.Blue);
        var dto = new UpdateTaskLabelDTO(1, "New Name", LabelColor.Red);

        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);
        _mockRepository.Setup(r => r.Update(It.IsAny<TaskLabel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _mockRepository.Verify(r => r.Update(It.Is<TaskLabel>(l => l.Name == "New Name" && l.LabelColor == LabelColor.Red), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenLabelNotFound_ShouldReturnFailure()
    {
        var dto = new UpdateTaskLabelDTO(999, "New Name", LabelColor.Red);

        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskLabel?)null);

        var result = await _service.UpdateAsync(dto, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Description.Should().Contain("999");
    }

    [Fact]
    public async Task UpdateAsync_WithNullDto_ShouldReturnFailure()
    {
        var result = await _service.UpdateAsync(null!, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.Update(It.IsAny<TaskLabel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── DeleteAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenLabelExists_ShouldReturnSuccess()
    {
        var label = CreateLabel();
        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(label);
        _mockRepository.Setup(r => r.Delete(It.IsAny<TaskLabel>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.Delete(label, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenLabelNotFound_ShouldReturnFailure()
    {
        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskLabel?)null);

        var result = await _service.DeleteAsync(999, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.Delete(It.IsAny<TaskLabel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ─── GetPagedAsync ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(1, 20)]
    public async Task GetPagedAsync_WithValidPagination_ShouldReturnCorrectPage(int page, int pageSize)
    {
        var totalLabels = 50;
        var labels = Enumerable.Range(1, totalLabels)
            .Select(i => CreateLabel($"Label {i}"))
            .ToList();

        _mockRepository.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(labels);

        var result = await _service.GetPagedAsync(page, pageSize);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(pageSize);
        result.Value.TotalCount.Should().Be(totalLabels);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    public async Task GetPagedAsync_WithInvalidPagination_ShouldReturnFailure(int page, int pageSize)
    {
        var result = await _service.GetPagedAsync(page, pageSize);

        result.IsSuccess.Should().BeFalse();
    }

    // ─── GetByFilterAsync ────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetByFilterAsync_FilterByUserId_ShouldReturnMatchingItems(int userId)
    {
        var filter = new TaskLabelFilterDTO(null, userId, null, null, null, null, null, null, null, null, 1, 10);
        var labels = Enumerable.Range(1, 3).Select(_ => CreateLabel(userId: userId)).ToList();

        _mockRepository
            .Setup(r => r.FindByFilter(It.Is<TaskLabelQueryFilter>(f => f.UserId == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((labels, labels.Count));

        var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(3);
        result.Value.Items.Should().AllSatisfy(l => l.UserId.Should().Be(userId));
    }

    [Fact]
    public async Task GetByFilterAsync_WhenNoResults_ShouldReturnEmptyListWithPagination()
    {
        var filter = new TaskLabelFilterDTO(null, null, null, "nomatch", null, null, null, null, null, null, 1, 10);

        _mockRepository
            .Setup(r => r.FindByFilter(It.IsAny<TaskLabelQueryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<TaskLabel>(), 0));

        var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
    }

    [Theory]
    [InlineData(LabelColor.Red)]
    [InlineData(LabelColor.Blue)]
    [InlineData(LabelColor.Green)]
    public async Task GetByFilterAsync_FilterByColor_ShouldReturnMatchingItems(LabelColor color)
    {
        var filter = new TaskLabelFilterDTO(null, null, null, null, color, null, null, null, null, null, 1, 10);
        var labels = new List<TaskLabel> { CreateLabel(color: color) };

        _mockRepository
            .Setup(r => r.FindByFilter(It.Is<TaskLabelQueryFilter>(f => f.LabelColor == color), It.IsAny<CancellationToken>()))
            .ReturnsAsync((labels, 1));

        var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().LabelColor.Should().Be(color);
    }

    // ─── CreateRangeAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateRangeAsync_WithValidDtos_ShouldReturnSuccessWithIds()
    {
        var dtos = new List<CreateTaskLabelDTO>
        {
            new("Work", LabelColor.Blue, 1),
            new("Personal", LabelColor.Green, 1),
            new("Urgent", LabelColor.Red, 1)
        };

        _mockRepository.Setup(r => r.CreateRange(It.IsAny<IEnumerable<TaskLabel>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateRangeAsync(dtos, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task CreateRangeAsync_WithEmptyList_ShouldReturnFailure()
    {
        var result = await _service.CreateRangeAsync(new List<CreateTaskLabelDTO>(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockRepository.Verify(r => r.CreateRange(It.IsAny<IEnumerable<TaskLabel>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRangeAsync_WithNullList_ShouldReturnFailure()
    {
        var result = await _service.CreateRangeAsync(null!, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    // ─── DeleteRangeAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteRangeAsync_WhenAllLabelsExist_ShouldReturnSuccess()
    {
        var label1 = CreateLabel("Work");
        var label2 = CreateLabel("Personal");

        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(label1);
        _mockRepository.Setup(r => r.GetById(2, It.IsAny<CancellationToken>())).ReturnsAsync(label2);
        _mockRepository.Setup(r => r.Update(It.IsAny<TaskLabel>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.DeleteRangeAsync(new[] { 1, 2 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRangeAsync_WhenSomeLabelsNotFound_ShouldReturnFailure()
    {
        var label = CreateLabel();
        _mockRepository.Setup(r => r.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(label);
        _mockRepository.Setup(r => r.GetById(999, It.IsAny<CancellationToken>())).ReturnsAsync((TaskLabel?)null);

        var result = await _service.DeleteRangeAsync(new[] { 1, 999 }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteRangeAsync_WithEmptyIds_ShouldReturnFailure()
    {
        var result = await _service.DeleteRangeAsync(new List<int>(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    // ─── CountAsync ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task CountAsync_WithoutPredicate_ShouldReturnTotalCount()
    {
        var labels = Enumerable.Range(1, 5).Select(i => CreateLabel($"Label {i}")).ToList();
        _mockRepository.Setup(r => r.GetAll(It.IsAny<CancellationToken>())).ReturnsAsync(labels);

        var result = await _service.CountAsync(null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(5);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
    {
        var labels = new List<TaskLabel>
        {
            CreateLabel("Blue Label", LabelColor.Blue),
            CreateLabel("Another Blue", LabelColor.Blue),
            CreateLabel("Red Label", LabelColor.Red)
        };
        _mockRepository.Setup(r => r.GetAll(It.IsAny<CancellationToken>())).ReturnsAsync(labels);

        var result = await _service.CountAsync(dto => dto.LabelColor == LabelColor.Blue, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2);
    }
}
