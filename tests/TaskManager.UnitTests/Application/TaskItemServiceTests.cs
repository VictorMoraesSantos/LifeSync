using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Filters;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.UnitTests.Application
{
    public class TaskItemServiceTests
    {
        private readonly Mock<ITaskItemRepository> _mockRepository;
        private readonly Mock<ILogger<TaskItemService>> _mockLogger;
        private readonly TaskItemService _service;

        public TaskItemServiceTests()
        {
            _mockRepository = new Mock<ITaskItemRepository>();
            _mockLogger = new Mock<ILogger<TaskItemService>>();
            _service = new TaskItemService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskExists_ReturnsSuccessResult()
        {
            // Arrange
            var taskId = 1;
            var taskItem = new TaskItem(
                "valid title",
                "valid description",
                Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                1);

            _mockRepository
                .Setup(r => r.GetById(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(taskItem);

            // Act
            var result = await _service.GetByIdAsync(taskId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result == null);
            Assert.Equal(taskItem.Title, result.Value.Title);
            Assert.Equal(taskItem.Description, result.Value.Description);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskNotExists_ReturnsFailResult()
        {
            // Arrange
            var taskId = 1;

            _mockRepository
                .Setup(r => r.GetById(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _service.GetByIdAsync(taskId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task GetAllAsync_WhenReturnIsEmpty_ShouldReturnEmptyListWithZeroCount()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TaskItem>());

            // Act
            var result = await _service.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.Empty(result.Value);
            Assert.Equal(0, result?.Value.Count());
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 20)]
        [InlineData(3, 30)]
        public async Task GetPagedAsync_WhenThereIsMoreItemsThanPageSize_ShouldReturnOnlyPageSizeItems(int page, int pageSize)
        {
            // Arrange
            var totalItems = 100;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i => new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1))
                .ToList();

            _mockRepository
                .Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeItems);

            // Act
            var result = await _service.GetPagedAsync(page, pageSize);

            // Assert
            Assert.Equal(pageSize, result.Value.Items.Count());
            Assert.Equal(totalItems, result.Value.TotalCount);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(-1, 10)]
        [InlineData(1, 0)]
        [InlineData(1, -5)]
        public async Task GetPagedAsync_WhenInvalidParameters_ShouldReturnFailResult(int page, int pageSize)
        {
            // Act
            var result = await _service.GetPagedAsync(page, pageSize);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value.Items);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetByFilterAsync_WhenFilterByUserId_ShouldReturnMatchingItems(int userId)
        {
            // Arrange
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: userId,
                TitleContains: null,
                Status: null,
                Priority: null,
                DueDate: null,
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: 1,
                PageSize: 10);

            var totalItems = 4;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i => new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), userId))
                .ToList();

            _mockRepository
                .Setup(r => r.FindByFilter(It.Is<TaskItemQueryFilter>(f => f.UserId == userId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems, totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.All(result.Value.Items, item => Assert.Equal(userId, item.UserId));
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }

        [Theory]
        [InlineData(Status.Pending)]
        [InlineData(Status.InProgress)]
        [InlineData(Status.Completed)]
        [InlineData(Status.Cancelled)]
        public async Task GetByFilterAsync_WhenFilterByStatus_ShouldReturnMatchingItems(Status status)
        {
            // Arrange
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: null,
                Status: status,
                Priority: null,
                DueDate: null,
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: 1,
                PageSize: 10);

            var totalItems = 4;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i => {
                    var item = new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1);
                    item.Update("valid title", "valid description", status, Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));
                    return item;
                })
                .ToList();

            _mockRepository
                .Setup(r => r.FindByFilter(It.Is<TaskItemQueryFilter>(f => f.Status.HasValue && f.Status.Value == status), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems, totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.All(result.Value.Items, item => Assert.Equal(status, item.Status));
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }

        [Theory]
        [InlineData(Priority.Urgent)]
        [InlineData(Priority.High)]
        [InlineData(Priority.Medium)]
        [InlineData(Priority.Low)]
        public async Task GetByFilterAsync_WhenFilterByPriority_ShouldReturnMatchingItems(Priority priority)
        {
            // Arrange
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: null,
                Status: null,
                Priority: priority,
                DueDate: null,
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: 1,
                PageSize: 10);

            var totalItems = 4;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i => {
                    var item = new TaskItem("valid title", "valid description", priority, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1);
                    return item;
                })
                .ToList();

            _mockRepository
                .Setup(r => r.FindByFilter(It.Is<TaskItemQueryFilter>(f => f.Priority.Value == priority), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems, totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.All(result.Value.Items, item => Assert.Equal(priority, item.Priority));
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }

        [Fact]
        public async Task GetByFilterAsync_WhenNoMatchingResults_ShouldReturnEmptyListWithPagination()
        {
            // Arrange
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: "nomatchingtitle",
                Status: null,
                Priority: null,
                DueDate: null,
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: 1,
                PageSize: 10);

            var totalItems = 10;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i => {
                    var item = new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1);
                    return item;
                })
                .ToList();

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value.Items);
            Assert.Equal(0, result.Value.Pagination.TotalItems);
        }
    }
}