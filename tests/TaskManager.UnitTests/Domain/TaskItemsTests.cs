using Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Errors;

namespace TaskManager.UnitTests.Domain
{
    public class TaskItemsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidTitle_ShouldThrowDomainException(string? invalidTitle)
        {
            // Act
            var result = Assert.Throws<DomainException>(() =>
                new TaskItem(
                    invalidTitle,
                    "valid description",
                    Priority.High,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                    1));

            // Assert
            Assert.Equal(TaskItemErrors.InvalidTitle.Description, result.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidDescription_ShouldThrowDomainException(string? invalidDescription)
        {
            // Act
            var result = Assert.Throws<DomainException>(() =>
                new TaskItem(
                    "valid title",
                    invalidDescription,
                    Priority.Medium,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                    1));

            // Assert
            Assert.Equal(TaskItemErrors.InvalidDescription.Description, result.Message);
        }

        [Fact]
        public void Create_WithInvalidPriority_ShouldThrowDomainException()
        {
            // Act
            var result = Assert.Throws<DomainException>(() =>
                new TaskItem(
                    "valid title",
                    "valid description",
                    (Priority)999,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                    1));

            // Assert
            Assert.Equal(TaskItemErrors.InvalidPriority.Description, result.Message);
        }

        [Fact]
        public void Create_WithDueDateInPast_ShouldThrowDomainException()
        {
            // Arrange
            var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            // Act
            var result = Assert.Throws<DomainException>(() =>
                new TaskItem(
                    "valid title",
                    "valid description",
                    Priority.Low,
                    pastDate,
                    1));

            // Assert
            Assert.Equal(TaskItemErrors.DueDateInPast.Description, result.Message);
        }

        [Fact]
        public void ChangeStatus_ShouldUpdateStatus()
        {
            var newStatus = Status.InProgress;

            // Arrange
            var taskItem = new TaskItem(
                "valid title",
                "valid description",
                Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow),
                1);

            // Act
            taskItem.ChangeStatus(newStatus);

            // Assert
            Assert.Equal(newStatus, taskItem.Status);
        }
    }
}
