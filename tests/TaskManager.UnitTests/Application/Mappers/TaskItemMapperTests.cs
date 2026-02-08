using FluentAssertions;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.UnitTests.Helpers.Builders;

namespace TaskManager.UnitTests.Application.Mappers;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class TaskItemMapperTests
{
    [Fact]
    public void ToDTO_WithValidEntity_MapsAllProperties()
    {
        // Arrange
        var taskItem = new TaskItem(
            "Test Task",
            "Test Description",
            Priority.High,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            1,
            null
        );

        // Set ID using reflection to simulate persisted entity
        typeof(TaskItem).GetProperty("Id")!.SetValue(taskItem, 123);

        // Act
        var dto = taskItem.ToDTO();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(123);
        dto.Title.Should().Be("Test Task");
        dto.Description.Should().Be("Test Description");
        dto.Priority.Should().Be(Priority.High);
        dto.Status.Should().Be(Status.Pending);
        dto.DueDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)));
        dto.UserId.Should().Be(1);
        dto.Labels.Should().BeEmpty();
    }

    [Fact]
    public void ToDTO_WithLabels_MapsLabelsCorrectly()
    {
        // Arrange
        var label1 = TaskLabelBuilder.Default().WithName("Work").WithColor(LabelColor.Blue).Build();
        var label2 = TaskLabelBuilder.Default().WithName("Urgent").WithColor(LabelColor.Red).Build();

        typeof(TaskLabel).GetProperty("Id")!.SetValue(label1, 1);
        typeof(TaskLabel).GetProperty("Id")!.SetValue(label2, 2);

        var taskItem = TaskItemBuilder.Default()
            .WithLabels(label1, label2)
            .Build();

        typeof(TaskItem).GetProperty("Id")!.SetValue(taskItem, 123);

        // Act
        var dto = taskItem.ToDTO();

        // Assert
        dto.Labels.Should().HaveCount(2);
        dto.Labels.Should().Contain(l => l.Name == "Work" && l.LabelColor == LabelColor.Blue);
        dto.Labels.Should().Contain(l => l.Name == "Urgent" && l.LabelColor == LabelColor.Red);
    }

    [Fact]
    public void ToDTO_WithAllStatuses_MapsStatusCorrectly()
    {
        // Arrange & Act & Assert
        foreach (Status status in Enum.GetValues(typeof(Status)))
        {
            var taskItem = TaskItemBuilder.Default().Build();
            taskItem.ChangeStatus(status);

            var dto = taskItem.ToDTO();

            dto.Status.Should().Be(status);
        }
    }

    [Fact]
    public void ToDTO_WithAllPriorities_MapsPriorityCorrectly()
    {
        // Arrange & Act & Assert
        foreach (Priority priority in Enum.GetValues(typeof(Priority)))
        {
            var taskItem = TaskItemBuilder.Default().WithPriority(priority).Build();

            var dto = taskItem.ToDTO();

            dto.Priority.Should().Be(priority);
        }
    }

    [Fact]
    public void ToDTO_WithUpdatedAt_MapsTimestampsCorrectly()
    {
        // Arrange
        var taskItem = TaskItemBuilder.Default().Build();
        typeof(TaskItem).GetProperty("Id")!.SetValue(taskItem, 123);

        // Update to set UpdatedAt
        taskItem.Update("Updated Title", "Updated Description", Status.InProgress, Priority.High, taskItem.DueDate);

        // Act
        var dto = taskItem.ToDTO();

        // Assert
        dto.CreatedAt.Should().NotBe(default);
        dto.UpdatedAt.Should().NotBeNull();
        dto.UpdatedAt.Should().BeAfter(dto.CreatedAt);
    }

    [Fact]
    public void ToEntity_WithValidCreateDTO_CreatesEntityWithAllProperties()
    {
        // Arrange
        var createDto = new CreateTaskItemDTO(
            "New Task",
            "New Description",
            Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            1,
            null
        );

        // Act
        var entity = createDto.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.Title.Should().Be("New Task");
        entity.Description.Should().Be("New Description");
        entity.Priority.Should().Be(Priority.Medium);
        entity.Status.Should().Be(Status.Pending);
        entity.DueDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));
        entity.UserId.Should().Be(1);
        entity.Labels.Should().BeEmpty();
    }

    [Fact]
    public void ToEntity_WithCreateDTO_SetsDefaultStatusToPending()
    {
        // Arrange
        var createDto = new CreateTaskItemDTO(
            "Task",
            "Description",
            Priority.Low,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            1,
            null
        );

        // Act
        var entity = createDto.ToEntity();

        // Assert
        entity.Status.Should().Be(Status.Pending);
    }

    [Fact]
    public void ToEntity_WithCreateDTO_InitializesEmptyLabelsCollection()
    {
        // Arrange
        var createDto = new CreateTaskItemDTO(
            "Task",
            "Description",
            Priority.Low,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            1,
            null
        );

        // Act
        var entity = createDto.ToEntity();

        // Assert
        entity.Labels.Should().NotBeNull();
        entity.Labels.Should().BeEmpty();
    }

    [Fact]
    public void ToDTO_RoundTrip_PreservesEssentialData()
    {
        // Arrange
        var createDto = new CreateTaskItemDTO(
            "Round Trip Task",
            "Round Trip Description",
            Priority.Urgent,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            5,
            null
        );

        // Act
        var entity = createDto.ToEntity();
        typeof(TaskItem).GetProperty("Id")!.SetValue(entity, 999);
        var resultDto = entity.ToDTO();

        // Assert
        resultDto.Title.Should().Be(createDto.Title);
        resultDto.Description.Should().Be(createDto.Description);
        resultDto.Priority.Should().Be(createDto.Priority);
        resultDto.DueDate.Should().Be(createDto.DueDate);
        resultDto.UserId.Should().Be(createDto.UserId);
    }
}
