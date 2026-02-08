using FluentAssertions;
using Core.Domain.Exceptions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Errors;
using TaskManager.UnitTests.Helpers.Builders;

namespace TaskManager.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class TaskLabelTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateEntity()
    {
        // Arrange
        var name = "Work";
        var color = LabelColor.Blue;
        var userId = 1;

        // Act
        var label = new TaskLabel(name, color, userId);

        // Assert
        label.Should().NotBeNull();
        label.Name.Should().Be(name);
        label.LabelColor.Should().Be(color);
        label.UserId.Should().Be(userId);
        label.Items.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange & Act
        var action = () => new TaskLabel(invalidName!, LabelColor.Blue, 1);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage(TaskLabelErrors.InvalidName.Description);
    }

    [Fact]
    public void Create_WithNameContainingWhitespace_ShouldTrimName()
    {
        // Arrange
        var nameWithSpaces = "  Work  ";

        // Act
        var label = new TaskLabel(nameWithSpaces, LabelColor.Blue, 1);

        // Assert
        label.Name.Should().Be("Work");
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();
        var newName = "Updated Label";
        var newColor = LabelColor.Red;

        // Act
        label.Update(newName, newColor);

        // Assert
        label.Name.Should().Be(newName);
        label.LabelColor.Should().Be(newColor);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Update_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();

        // Act
        var action = () => label.Update(invalidName!, LabelColor.Blue);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage(TaskLabelErrors.InvalidName.Description);
    }

    [Fact]
    public void Update_WhenUpdateEntity_ShouldMarkAsUpdated()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();
        var originalCreatedAt = label.CreatedAt;

        // Small delay to ensure timestamp difference
        Thread.Sleep(10);

        // Act
        label.Update("New Name", LabelColor.Green);

        // Assert
        label.UpdatedAt.Should().NotBeNull();
        label.UpdatedAt.Should().BeAfter(originalCreatedAt);
    }

    [Fact]
    public void AddTaskItem_WithValidItem_ShouldAddToCollection()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();
        var taskItem = TaskItemBuilder.Default().Build();

        // Act
        label.AddTaskItem(taskItem);

        // Assert
        label.Items.Should().ContainSingle();
        label.Items.First().Should().Be(taskItem);
    }

    [Fact]
    public void AddTaskItem_WithNullItem_ShouldThrowDomainException()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();

        // Act
        var action = () => label.AddTaskItem(null!);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage(TaskLabelErrors.NullItem.Description);
    }

    [Fact]
    public void AddTaskItem_WithDuplicateItem_ShouldThrowDomainException()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();
        var taskItem = TaskItemBuilder.Default().Build();
        label.AddTaskItem(taskItem);

        // Act
        var action = () => label.AddTaskItem(taskItem);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage(TaskLabelErrors.DuplicateItem.Description);
    }

    [Fact]
    public void AddTaskItem_WithMultipleItems_ShouldAddAllToCollection()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();
        var item1 = TaskItemBuilder.Default().WithTitle("Task 1").Build();
        var item2 = TaskItemBuilder.Default().WithTitle("Task 2").Build();
        var item3 = TaskItemBuilder.Default().WithTitle("Task 3").Build();

        // Set different IDs using reflection to simulate distinct entities
        typeof(TaskItem).GetProperty("Id")!.SetValue(item1, 1);
        typeof(TaskItem).GetProperty("Id")!.SetValue(item2, 2);
        typeof(TaskItem).GetProperty("Id")!.SetValue(item3, 3);

        // Act
        label.AddTaskItem(item1);
        label.AddTaskItem(item2);
        label.AddTaskItem(item3);

        // Assert
        label.Items.Should().HaveCount(3);
        label.Items.Should().Contain(new[] { item1, item2, item3 });
    }

    [Fact]
    public void RemoveTaskItem_WithExistingItem_ShouldRemoveFromCollection()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();
        var taskItem = TaskItemBuilder.Default().Build();
        label.AddTaskItem(taskItem);

        // Act
        label.RemoveTaskItem(taskItem);

        // Assert
        label.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveTaskItem_WithNullItem_ShouldThrowDomainException()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();

        // Act
        var action = () => label.RemoveTaskItem(null!);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage(TaskLabelErrors.NullItem.Description);
    }

    [Fact]
    public void RemoveTaskItem_WithNonExistentItem_ShouldThrowDomainException()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();
        var taskItem = TaskItemBuilder.Default().Build();

        // Act
        var action = () => label.RemoveTaskItem(taskItem);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage(TaskLabelErrors.ItemNotFound.Description);
    }

    [Fact]
    public void RemoveTaskItem_FromMultipleItems_ShouldRemoveOnlySpecifiedItem()
    {
        // Arrange
        var label = TaskLabelBuilder.Default().Build();
        var item1 = TaskItemBuilder.Default().WithTitle("Task 1").Build();
        var item2 = TaskItemBuilder.Default().WithTitle("Task 2").Build();
        var item3 = TaskItemBuilder.Default().WithTitle("Task 3").Build();

        // Set different IDs using reflection to simulate distinct entities
        typeof(TaskItem).GetProperty("Id")!.SetValue(item1, 1);
        typeof(TaskItem).GetProperty("Id")!.SetValue(item2, 2);
        typeof(TaskItem).GetProperty("Id")!.SetValue(item3, 3);

        label.AddTaskItem(item1);
        label.AddTaskItem(item2);
        label.AddTaskItem(item3);

        // Act
        label.RemoveTaskItem(item2);

        // Assert
        label.Items.Should().HaveCount(2);
        label.Items.Should().Contain(new[] { item1, item3 });
        label.Items.Should().NotContain(item2);
    }
}
