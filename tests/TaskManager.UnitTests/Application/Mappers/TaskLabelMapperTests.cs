using FluentAssertions;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.UnitTests.Helpers.Builders;

namespace TaskManager.UnitTests.Application.Mappers;

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class TaskLabelMapperTests
{
    [Fact]
    public void ToDTO_WithValidEntity_MapsAllProperties()
    {
        // Arrange
        var taskLabel = new TaskLabel("Work", LabelColor.Blue, 1);

        // Set ID using reflection to simulate persisted entity
        typeof(TaskLabel).GetProperty("Id")!.SetValue(taskLabel, 42);

        // Act
        var dto = taskLabel.ToDTO();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(42);
        dto.Name.Should().Be("Work");
        dto.LabelColor.Should().Be(LabelColor.Blue);
        dto.UserId.Should().Be(1);
    }

    [Fact]
    public void ToDTO_WithAllColors_MapsColorCorrectly()
    {
        // Arrange & Act & Assert
        foreach (LabelColor color in Enum.GetValues(typeof(LabelColor)))
        {
            var taskLabel = TaskLabelBuilder.Default().WithColor(color).Build();

            var dto = taskLabel.ToDTO();

            dto.LabelColor.Should().Be(color);
        }
    }

    [Fact]
    public void ToDTO_WithUpdatedAt_MapsTimestampsCorrectly()
    {
        // Arrange
        var taskLabel = TaskLabelBuilder.Default().Build();
        typeof(TaskLabel).GetProperty("Id")!.SetValue(taskLabel, 42);

        // Update to set UpdatedAt
        taskLabel.Update("Updated Label", LabelColor.Green);

        // Act
        var dto = taskLabel.ToDTO();

        // Assert
        dto.CreatedAt.Should().NotBe(default);
        dto.UpdatedAt.Should().NotBeNull();
        dto.UpdatedAt.Should().BeAfter(dto.CreatedAt);
    }

    [Fact]
    public void ToEntity_WithValidCreateDTO_CreatesEntityWithAllProperties()
    {
        // Arrange
        var createDto = new CreateTaskLabelDTO("Personal", LabelColor.Green, 2);

        // Act
        var entity = createDto.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.Name.Should().Be("Personal");
        entity.LabelColor.Should().Be(LabelColor.Green);
        entity.UserId.Should().Be(2);
    }

    [Fact]
    public void ToEntity_WithAllColors_CreatesEntityWithCorrectColor()
    {
        // Arrange & Act & Assert
        foreach (LabelColor color in Enum.GetValues(typeof(LabelColor)))
        {
            var createDto = new CreateTaskLabelDTO("Label", color, 1);

            var entity = createDto.ToEntity();

            entity.LabelColor.Should().Be(color);
        }
    }

    [Fact]
    public void ToEntity_WithCreateDTO_InitializesEmptyItemsCollection()
    {
        // Arrange
        var createDto = new CreateTaskLabelDTO("Label", LabelColor.Blue, 1);

        // Act
        var entity = createDto.ToEntity();

        // Assert
        entity.Items.Should().NotBeNull();
        entity.Items.Should().BeEmpty();
    }

    [Fact]
    public void ToDTO_RoundTrip_PreservesEssentialData()
    {
        // Arrange
        var createDto = new CreateTaskLabelDTO("Round Trip Label", LabelColor.Purple, 7);

        // Act
        var entity = createDto.ToEntity();
        typeof(TaskLabel).GetProperty("Id")!.SetValue(entity, 888);
        var resultDto = entity.ToDTO();

        // Assert
        resultDto.Name.Should().Be(createDto.Name);
        resultDto.LabelColor.Should().Be(createDto.LabelColor);
        resultDto.UserId.Should().Be(createDto.UserId);
    }

    [Fact]
    public void ToDTO_WithDifferentUserIds_MapsCorrectly()
    {
        // Arrange & Act & Assert
        for (int userId = 1; userId <= 5; userId++)
        {
            var taskLabel = TaskLabelBuilder.Default().WithUserId(userId).Build();

            var dto = taskLabel.ToDTO();

            dto.UserId.Should().Be(userId);
        }
    }

    [Fact]
    public void ToDTO_WithLongName_MapsCorrectly()
    {
        // Arrange
        var longName = new string('A', 50);
        var taskLabel = new TaskLabel(longName, LabelColor.Orange, 1);
        typeof(TaskLabel).GetProperty("Id")!.SetValue(taskLabel, 123);

        // Act
        var dto = taskLabel.ToDTO();

        // Assert
        dto.Name.Should().Be(longName);
        dto.Name.Length.Should().Be(50);
    }
}
