using Core.Domain.Exceptions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Errors;

namespace TaskManager.UnitTests.Domain
{
    public class TaskItemsTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateEntity()
        {
            //Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
            var validUserId = 1;

            //Act
            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            Assert.NotNull(taskItem);
            Assert.Equal(taskItem.Title, validTitle);
            Assert.Equal(taskItem.Description, validDescription);
            Assert.Equal(taskItem.Priority, validPriority);
            Assert.Equal(taskItem.DueDate, validDueDate);
            Assert.Equal(taskItem.UserId, validUserId);
        }

        [Fact]
        public void Create_WhenCreateEntity_StatusShouldBePending()
        {
            //Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
            var validUserId = 1;
            var expectedStatus = Status.Pending;

            //Act
            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            //Assert
            Assert.NotNull(taskItem.Status);
            Assert.Equal(expectedStatus, taskItem.Status);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidTitle_ShouldThrowDomainException(string? invalidTitle)
        {
            //Arrange
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
            var validUserId = 1;

            // Act
            var result = Assert.Throws<DomainException>(() =>
                new TaskItem(
                    invalidTitle,
                    validDescription,
                    validPriority,
                    validDueDate,
                    validUserId));

            // Assert
            Assert.Equal(TaskItemErrors.InvalidTitle.Description, result.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidDescription_ShouldThrowDomainException(string? invalidDescription)
        {
            //Arrange
            var validTitle = "valid title";
            var validPriority = Priority.Medium;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
            var validUserId = 1;

            // Act
            var result = Assert.Throws<DomainException>(() =>
                new TaskItem(
                    validTitle,
                    invalidDescription,
                    validPriority,
                    validDueDate,
                    validUserId));

            // Assert
            Assert.Equal(TaskItemErrors.InvalidDescription.Description, result.Message);
        }

        [Theory]
        [InlineData(999)]
        [InlineData(9)]
        [InlineData(0)]
        [InlineData(5)]
        public void Create_WithInvalidPriority_ShouldThrowDomainException(int invalidPriority)
        {
            //Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
            var validUserId = 1;

            // Act
            var result = Assert.Throws<DomainException>(() =>
                new TaskItem(
                    validTitle,
                    validDescription,
                    (Priority)(invalidPriority),
                    validDueDate,
                    validUserId));

            // Assert
            Assert.Equal(TaskItemErrors.InvalidPriority.Description, result.Message);
        }

        [Fact]
        public void Create_WithDueDateInPast_ShouldThrowDomainException()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var pastDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            // Act
            var result = Assert.Throws<DomainException>(() =>
                new TaskItem(
                    validTitle,
                    validDescription,
                    validPriority,
                    pastDueDate,
                    validUserId));

            // Assert
            Assert.Equal(TaskItemErrors.DueDateInPast.Description, result.Message);
        }

        [Fact]
        public void ChangeStatus_ShouldUpdateStatus()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var newStatus = Status.InProgress;

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            // Act
            taskItem.ChangeStatus(newStatus);

            // Assert
            Assert.Equal(newStatus, taskItem.Status);
        }

        public void Update_WithValidParameters_ShouldUpdateProperties()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var newTitle = "new title";
            var newDescription = "new description";
            var newStatus = Status.InProgress;
            var newPriority = Priority.Urgent;
            var newDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            // Act
            taskItem.Update(newTitle, newDescription, newStatus, newPriority, newDueDate);
            // Assert

            Assert.Equal(newTitle, taskItem.Title);
            Assert.Equal(newDescription, taskItem.Description);
            Assert.Equal(newStatus, taskItem.Status);
            Assert.Equal(newPriority, taskItem.Priority);
            Assert.Equal(newDueDate, taskItem.DueDate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidTitle_ShouldThrowDomainException(string? invalidTitle)
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var newDescription = "new description";
            var newStatus = Status.InProgress;
            var newPriority = Priority.Urgent;
            var newDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

            // Act
            var result = Record.Exception(() =>
                taskItem.Update(
                    invalidTitle,
                    newDescription,
                    newStatus,
                    newPriority,
                    newDueDate));

            // Assert
            Assert.IsType<DomainException>(result);
            Assert.Equal(TaskItemErrors.InvalidTitle.Description, result?.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidDescription_ShouldThrowDomainException(string? invalidDescription)
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var newTitle = "new title";
            var newStatus = Status.InProgress;
            var newPriority = Priority.Urgent;
            var newDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

            // Act
            var result = Record.Exception(() =>
                taskItem.Update(
                    newTitle,
                    invalidDescription,
                    newStatus,
                    newPriority,
                    newDueDate));
            // Assert
            Assert.IsType<DomainException>(result);
            Assert.Equal(TaskItemErrors.InvalidDescription.Description, result?.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(5)]
        public void Update_WithInvalidPriority_ShouldThrowDomainException(int invalidPriority)
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var newTitle = "new title";
            var newDescription = "new description";
            var newStatus = Status.InProgress;
            var newDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

            // Act
            var result = Record.Exception(() =>
                taskItem.Update(
                    newTitle,
                    newDescription,
                    newStatus,
                    (Priority)(invalidPriority),
                    newDueDate));

            // Assert
            Assert.IsType<DomainException>(result);
            Assert.Equal(TaskItemErrors.InvalidPriority.Description, result?.Message);
        }


        [Fact]
        public void Update_WithDueDateInPast_ShouldThrowDomainException()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var newTitle = "new title";
            var newDescription = "new description";
            var newStatus = Status.InProgress;
            var newPriority = Priority.Urgent;
            var pastDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            // Act
            var result = Record.Exception(() =>
                taskItem.Update(
                    newTitle,
                    newDescription,
                    newStatus,
                    newPriority,
                    pastDueDate));

            // Assert
            Assert.IsType<DomainException>(result);
            Assert.Equal(TaskItemErrors.DueDateInPast.Description, result?.Message);
        }

        [Fact]
        public void Update_WhenUpdateEntity_ShouldMaskAsUpdated()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var newTitle = "new title";
            var newDescription = "new description";
            var newStatus = Status.InProgress;
            var newPriority = Priority.Urgent;
            var newDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var createdAt = taskItem.CreatedAt;

            // Act
            taskItem.Update(newTitle, newDescription, newStatus, newPriority, newDueDate);

            // Assert
            Assert.NotNull(taskItem.UpdatedAt);
            Assert.True(taskItem.UpdatedAt > createdAt);
        }

        [Fact]
        public void AddLabel_ShouldAddLabel()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var validLabelTitle = "valid title";
            var validLabelColor = LabelColor.Red;

            var taskLabel = new TaskLabel(
                validLabelTitle,
                validLabelColor,
                validUserId,
                taskItem.Id);

            //Act
            taskItem.AddLabel(taskLabel);

            //Assert
            Assert.Contains(taskLabel, taskItem.Labels);
        }

        [Fact]
        public void AddLabel_WhitNullLabel_ShouldThrowDomainException()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            //Act
            var result = Record.Exception(() => taskItem.AddLabel(null));

            //Assert
            Assert.IsType<DomainException>(result);
            Assert.Equal(TaskItemErrors.NullLabel.Description, result?.Message);
        }

        [Fact]
        public void AddLabel_WhitExistingLabel_ShouldThrowDomainException()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var validLabelTitle = "valid title";
            var validLabelColor = LabelColor.Red;

            var taskLabel = new TaskLabel(
                validLabelTitle,
                validLabelColor,
                validUserId,
                taskItem.Id);

            //Act
            taskItem.AddLabel(taskLabel);
            var result = Record.Exception(() => taskItem.AddLabel(taskLabel));

            //Assert
            Assert.IsType<DomainException>(result);
            Assert.Equal(TaskItemErrors.DuplicateLabel.Description, result?.Message);
        }

        [Fact]
        public void RemoveLabel_ShouldRemoveLabel()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var validLabelTitle = "valid title";
            var validLabelColor = LabelColor.Red;

            var taskLabel = new TaskLabel(
                validLabelTitle,
                validLabelColor,
                validUserId,
                taskItem.Id);

            //Act
            taskItem.AddLabel(taskLabel);
            taskItem.RemoveLabel(taskLabel);

            //Assert
            Assert.DoesNotContain(taskLabel, taskItem.Labels);
        }

        [Fact]
        public void RemoveLabel_WhitNullLabel_ShouldThrowDomainException()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            //Act
            var result = Record.Exception(() => taskItem.RemoveLabel(null));

            //Assert
            Assert.IsType<DomainException>(result);
            Assert.Equal(TaskItemErrors.NullLabel.Description, result?.Message);
        }

        [Fact]
        public void RemoveLabel_WhitNonExistingLabel_ShouldThrowDomainException()
        {
            // Arrange
            var validTitle = "valid title";
            var validDescription = "valid description";
            var validPriority = Priority.Medium;
            var validUserId = 1;
            var validDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var taskItem = new TaskItem(
                validTitle,
                validDescription,
                validPriority,
                validDueDate,
                validUserId);

            var validLabelTitle = "valid title";
            var validLabelColor = LabelColor.Red;

            var taskLabel = new TaskLabel(
                validLabelTitle,
                validLabelColor,
                validUserId,
                taskItem.Id);

            //Act
            var result = Record.Exception(() => taskItem.RemoveLabel(taskLabel));

            //Assert
            Assert.IsType<DomainException>(result);
            Assert.Equal(TaskItemErrors.LabelNotFound.Description, result?.Message);
        }
    }
}
