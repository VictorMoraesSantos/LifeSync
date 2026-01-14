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
        private readonly Mock<ITaskLabelRepository> _mockLabelRepository;
        private readonly Mock<ILogger<TaskItemService>> _mockLogger;
        private readonly TaskItemService _service;

        public TaskItemServiceTests()
        {
            _mockRepository = new Mock<ITaskItemRepository>();
            _mockLabelRepository = new Mock<ITaskLabelRepository>();
            _mockLogger = new Mock<ILogger<TaskItemService>>();
            _service = new TaskItemService(_mockRepository.Object, _mockLogger.Object, _mockLabelRepository.Object);
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
                1,
                null);

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
                .Select(i => new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null))
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
                .Select(i => new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), userId, null))
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
                .Select(i =>
                {
                    var item = new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null);
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
                .Select(i =>
                {
                    var item = new TaskItem("valid title", "valid description", priority, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null);
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
                .Select(i =>
                {
                    var item = new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null);
                    return item;
                })
                .ToList();

            _mockRepository
                .Setup(r => r.FindByFilter(
                    It.Is<TaskItemQueryFilter>(q => q.TitleContains == filter.TitleContains),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<TaskItem>(), 0));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value.Items);
            Assert.Equal(0, result.Value.Pagination.TotalItems);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 10)]
        [InlineData(3, 10)]
        [InlineData(4, 20)]
        [InlineData(5, 20)]
        public async Task GetByFilterAsync_WhenFilterWithPagination_ShouldReturnCorrectPageInfo(int page, int pageSize)
        {
            // Arrange
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: "valid title",
                Status: null,
                Priority: null,
                DueDate: null,
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: page,
                PageSize: pageSize);

            var totalItems = 100;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i =>
                {
                    var item = new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null);
                    return item;
                })
                .ToList();

            _mockRepository
                .Setup(r => r.FindByFilter(It.IsAny<TaskItemQueryFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems, totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(page, result.Value.Pagination.CurrentPage);
            Assert.Equal(pageSize, result.Value.Pagination.PageSize);
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }

        [Fact]
        public async Task GetByFilter_WithValidDueDate_ShouldReturnMatchingItem()
        {
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: null,
                Status: null,
                Priority: null,
                DueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: 1,
                PageSize: 10);
            var totalItems = 2;
            var pageSize = 10;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i =>
                {
                    var item = new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
                    return item;
                })
                .ToList();
            _mockRepository
                .Setup(r => r.FindByFilter(It.Is<TaskItemQueryFilter>(f => f.DueDate.HasValue && f.DueDate.Value == filter.DueDate), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems, totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.All(result.Value.Items, item => Assert.Equal(filter.DueDate, item.DueDate));
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }

        [Fact]
        public async Task GetByFilter_WithValidCreatedAtDate_ShouldReturnMatchingItem()
        {
            var targetDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: null,
                Status: null,
                Priority: null,
                DueDate: null,
                LabelId: null,
                CreatedAt: targetDate,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: 1,
                PageSize: 10);
            var totalItems = 3;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i =>
                {
                    var item = new TaskItem("valid title", "valid description", Priority.Medium, targetDate, 1, null);
                    return item;
                })
                .ToList();
            _mockRepository
                .Setup(r => r.FindByFilter(It.Is<TaskItemQueryFilter>(f => f.CreatedAt.HasValue && f.CreatedAt.Value == filter.CreatedAt), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems, totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.All(result.Value.Items, item => Assert.Equal(targetDate, DateOnly.FromDateTime(item.CreatedAt)));
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }

        [Fact]
        public async Task GetByFilter_WithValidUpdatedAtDate_ShouldReturnMatchingItem()
        {
            var targetDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: null,
                Status: null,
                Priority: null,
                DueDate: null,
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: targetDate,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: 1,
                PageSize: 10);
            var totalItems = 2;
            var fakeItems = Enumerable.Range(1, totalItems)
                .Select(i =>
                {
                    var item = new TaskItem("valid title", "valid description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null);
                    item.Update("valid title", "valid description", Status.Pending, Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));
                    return item;
                })
                .ToList();
            _mockRepository
                .Setup(r => r.FindByFilter(It.Is<TaskItemQueryFilter>(f => f.UpdatedAt.HasValue && f.UpdatedAt.Value == filter.UpdatedAt), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems, totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.All(result.Value.Items, item => Assert.Equal(targetDate, DateOnly.FromDateTime(item.UpdatedAt!.Value)));
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }

        [Fact]
        public async Task GetByFilter_WithValidSort_ByTitleAsc_ShouldReturnSortedItems()
        {
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: null,
                Status: null,
                Priority: null,
                DueDate: null,
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: "Title",
                SortDesc: false,
                Page: 1,
                PageSize: 10);
            var totalItems = 5;
            var fakeItems = new List<TaskItem>
            {
                new TaskItem("Bravo", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null),
                new TaskItem("Alpha", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null),
                new TaskItem("Delta", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null),
                new TaskItem("Charlie", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null),
                new TaskItem("Echo", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null)
            };
            _mockRepository
                .Setup(r => r.FindByFilter(It.Is<TaskItemQueryFilter>(f => f.SortBy == filter.SortBy && f.SortDesc == filter.SortDesc), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems.OrderBy(t => t.Title).ToList(), totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var titles = result.Value.Items.Select(i => i.Title).ToList();
            var sortedTitles = titles.OrderBy(t => t).ToList();
            Assert.Equal(sortedTitles, titles);
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }

        public async Task GetByFilter_WhenSortDescending_ThenReturnsItemsInDescendingOrder()
        {
            var filter = new TaskItemFilterDTO(
                Id: null,
                UserId: null,
                TitleContains: null,
                Status: null,
                Priority: null,
                DueDate: null,
                LabelId: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: "Title",
                SortDesc: true,
                Page: 1,
                PageSize: 10);
            var totalItems = 5;
            var fakeItems = new List<TaskItem>
            {
                new TaskItem("Bravo", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null),
                new TaskItem("Alpha", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null),
                new TaskItem("Delta", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null),
                new TaskItem("Charlie", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null),
                new TaskItem("Echo", "Description", Priority.Medium, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, null)
            };
            _mockRepository
                .Setup(r => r.FindByFilter(It.Is<TaskItemQueryFilter>(f => f.SortBy == filter.SortBy && f.SortDesc == filter.SortDesc), It.IsAny<CancellationToken>()))
                .ReturnsAsync((fakeItems.OrderByDescending(t => t.Title).ToList(), totalItems));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var titles = result.Value.Items.Select(i => i.Title).ToList();
            var sortedTitles = titles.OrderByDescending(t => t).ToList();
            Assert.Equal(sortedTitles, titles);
            Assert.Equal(totalItems, result.Value.Pagination.TotalItems);
        }
    }
}