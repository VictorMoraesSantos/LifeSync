using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Persistence.Repositories;
using TaskManager.Infrastructure.Services;
using TaskManager.IntegrationTests.Fixtures;

namespace TaskManager.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class TaskItemServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly TaskItemService _service;
        private readonly TaskItemRepository _taskItemRepository;
        private readonly TaskLabelRepository _taskLabelRepository;

        public TaskItemServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _taskItemRepository = new TaskItemRepository(_fixture.DbContext);
            _taskLabelRepository = new TaskLabelRepository(_fixture.DbContext);
            _service = new TaskItemService(
                _taskItemRepository,
                NullLogger<TaskItemService>.Instance,
                _taskLabelRepository);
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_WithValidDTO_ReturnsSuccessWithId()
        {
            // Arrange
            var dto = new CreateTaskItemDTO(
                "Service Test Task",
                "Description",
                Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                1,
                null);

            // Act
            var result = await _service.CreateAsync(dto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateAsync_WithLabels_CreatesTaskWithLabelsAssociated()
        {
            // Arrange
            var label1 = new TaskLabel("Label 1", LabelColor.Red, 1);
            var label2 = new TaskLabel("Label 2", LabelColor.Blue, 1);
            await _taskLabelRepository.Create(label1);
            await _taskLabelRepository.Create(label2);

            var dto = new CreateTaskItemDTO(
                "Task With Labels",
                "Description",
                Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                1,
                new List<int> { label1.Id, label2.Id });

            // Act
            var result = await _service.CreateAsync(dto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var taskResult = await _service.GetByIdAsync(result.Value, CancellationToken.None);
            taskResult.IsSuccess.Should().BeTrue();
            taskResult.Value.Labels.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistingLabelId_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateTaskItemDTO(
                "Task",
                "Description",
                Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                1,
                new List<int> { 99999 });

            // Act
            var result = await _service.CreateAsync(dto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithNullDTO_ReturnsFailure()
        {
            // Act
            var result = await _service.CreateAsync(null!, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region CreateRangeAsync

        [Fact]
        public async Task CreateRangeAsync_WithValidDTOs_ReturnsSuccessWithIds()
        {
            // Arrange
            var dtos = Enumerable.Range(1, 3).Select(i => new CreateTaskItemDTO(
                $"Batch Task {i}",
                $"Description {i}",
                Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                1,
                null));

            // Act
            var result = await _service.CreateRangeAsync(dtos, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(3);
        }

        [Fact]
        public async Task CreateRangeAsync_WithEmptyCollection_ReturnsFailure()
        {
            // Act
            var result = await _service.CreateRangeAsync(Enumerable.Empty<CreateTaskItemDTO>(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsSuccessWithDTO()
        {
            // Arrange
            var dto = new CreateTaskItemDTO("Get Me", "Description", Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var createResult = await _service.CreateAsync(dto, CancellationToken.None);

            // Act
            var result = await _service.GetByIdAsync(createResult.Value, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Get Me");
            result.Value.Priority.Should().Be(Priority.High);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsFailure()
        {
            // Act
            var result = await _service.GetByIdAsync(99999, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_WithMultipleItems_ReturnsAllDTOs()
        {
            // Arrange
            for (int i = 1; i <= 3; i++)
            {
                await _service.CreateAsync(new CreateTaskItemDTO(
                    $"Task {i}", "Desc", Priority.Medium,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 1, null), CancellationToken.None);
            }

            // Act
            var result = await _service.GetAllAsync(CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllAsync_WithNoItems_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllAsync(CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        #endregion

        #region GetByFilterAsync

        [Fact]
        public async Task GetByFilterAsync_WithStatusFilter_ReturnsFilteredResults()
        {
            // Arrange
            var createDto1 = new CreateTaskItemDTO("Pending Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var createDto2 = new CreateTaskItemDTO("Another Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            await _service.CreateAsync(createDto1, CancellationToken.None);
            var result2 = await _service.CreateAsync(createDto2, CancellationToken.None);

            // Change status of second task
            await _service.UpdateAsync(new UpdateTaskItemDTO(
                result2.Value, "Another Task", "Desc", Status.Completed, Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))), CancellationToken.None);

            var filter = new TaskItemFilterDTO(
                null, 1, null, Status.Pending, null, null, null,
                null, null, null, null, null, null, null);

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.Items.First().Status.Should().Be(Status.Pending);
        }

        [Fact]
        public async Task GetByFilterAsync_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 10; i++)
            {
                await _service.CreateAsync(new CreateTaskItemDTO(
                    $"Task {i:D2}", "Desc", Priority.Medium,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 1, null), CancellationToken.None);
            }

            var filter = new TaskItemFilterDTO(
                null, 1, null, null, null, null, null,
                null, null, null, null, null, 1, 3);

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(3);
            result.Value.Pagination.TotalItems.Should().Be(10);
            result.Value.Pagination.TotalPages.Should().Be(4);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WithValidDTO_ReturnsSuccess()
        {
            // Arrange
            var createDto = new CreateTaskItemDTO("Original", "Desc", Priority.Low,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var createResult = await _service.CreateAsync(createDto, CancellationToken.None);

            var updateDto = new UpdateTaskItemDTO(
                createResult.Value, "Updated Title", "Updated Desc",
                Status.InProgress, Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));

            // Act
            var result = await _service.UpdateAsync(updateDto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value, CancellationToken.None);
            getResult.Value.Title.Should().Be("Updated Title");
            getResult.Value.Status.Should().Be(Status.InProgress);
            getResult.Value.Priority.Should().Be(Priority.High);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingId_ReturnsFailure()
        {
            // Arrange
            var updateDto = new UpdateTaskItemDTO(99999, "Title", "Desc",
                Status.Pending, Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)));

            // Act
            var result = await _service.UpdateAsync(updateDto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region AddLabelAsync / RemoveLabelAsync

        [Fact]
        public async Task AddLabelAsync_WithValidIds_AddsLabelToTask()
        {
            // Arrange
            var label = new TaskLabel("New Label", LabelColor.Green, 1);
            await _taskLabelRepository.Create(label);

            var createDto = new CreateTaskItemDTO("Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var createResult = await _service.CreateAsync(createDto, CancellationToken.None);

            var updateLabelsDto = new UpdateLabelsDTO(createResult.Value, new List<int> { label.Id });

            // Act
            var result = await _service.AddLabelAsync(updateLabelsDto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value, CancellationToken.None);
            getResult.Value.Labels.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddLabelAsync_WithAlreadyExistingLabel_SkipsWithoutError()
        {
            // Arrange
            var label = new TaskLabel("Existing Label", LabelColor.Red, 1);
            await _taskLabelRepository.Create(label);

            var createDto = new CreateTaskItemDTO("Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1,
                new List<int> { label.Id });
            var createResult = await _service.CreateAsync(createDto, CancellationToken.None);

            var updateLabelsDto = new UpdateLabelsDTO(createResult.Value, new List<int> { label.Id });

            // Act
            var result = await _service.AddLabelAsync(updateLabelsDto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value, CancellationToken.None);
            getResult.Value.Labels.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddLabelAsync_WithNonExistingTaskId_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateLabelsDTO(99999, new List<int> { 1 });

            // Act
            var result = await _service.AddLabelAsync(dto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveLabelAsync_WithExistingLabel_RemovesLabelFromTask()
        {
            // Arrange
            var label = new TaskLabel("Remove Me", LabelColor.Yellow, 1);
            await _taskLabelRepository.Create(label);

            var createDto = new CreateTaskItemDTO("Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1,
                new List<int> { label.Id });
            var createResult = await _service.CreateAsync(createDto, CancellationToken.None);

            var updateLabelsDto = new UpdateLabelsDTO(createResult.Value, new List<int> { label.Id });

            // Act
            var result = await _service.RemoveLabelAsync(updateLabelsDto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value, CancellationToken.None);
            getResult.Value.Labels.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveLabelAsync_WithNonExistingTaskId_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateLabelsDTO(99999, new List<int> { 1 });

            // Act
            var result = await _service.RemoveLabelAsync(dto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_WithExistingId_ReturnsSuccessAndRemovesEntity()
        {
            // Arrange
            var createDto = new CreateTaskItemDTO("Delete Me", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null);
            var createResult = await _service.CreateAsync(createDto, CancellationToken.None);

            // Act
            var result = await _service.DeleteAsync(createResult.Value, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value, CancellationToken.None);
            getResult.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ReturnsFailure()
        {
            // Act
            var result = await _service.DeleteAsync(99999, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region DeleteRangeAsync

        [Fact]
        public async Task DeleteRangeAsync_WithExistingIds_SoftDeletesAll()
        {
            // Arrange
            var ids = new List<int>();
            for (int i = 1; i <= 3; i++)
            {
                var createResult = await _service.CreateAsync(new CreateTaskItemDTO(
                    $"Delete Task {i}", "Desc", Priority.Medium,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 1, null), CancellationToken.None);
                ids.Add(createResult.Value);
            }

            // Act
            var result = await _service.DeleteRangeAsync(ids, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteRangeAsync_WithSomeNonExistingIds_ReturnsFailure()
        {
            // Arrange
            var createResult = await _service.CreateAsync(new CreateTaskItemDTO(
                "Task", "Desc", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null), CancellationToken.None);

            var ids = new List<int> { createResult.Value, 99999 };

            // Act
            var result = await _service.DeleteRangeAsync(ids, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteRangeAsync_WithEmptyList_ReturnsFailure()
        {
            // Act
            var result = await _service.DeleteRangeAsync(Enumerable.Empty<int>(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetPagedAsync

        [Fact]
        public async Task GetPagedAsync_WithValidParameters_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 8; i++)
            {
                await _service.CreateAsync(new CreateTaskItemDTO(
                    $"Task {i}", "Desc", Priority.Medium,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 1, null), CancellationToken.None);
            }

            // Act
            var result = await _service.GetPagedAsync(1, 3, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(3);
            result.Value.TotalCount.Should().Be(8);
        }

        [Fact]
        public async Task GetPagedAsync_WithInvalidParameters_ReturnsFailure()
        {
            // Act
            var result = await _service.GetPagedAsync(0, -1, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region CountAsync

        [Fact]
        public async Task CountAsync_WithItems_ReturnsCorrectCount()
        {
            // Arrange
            for (int i = 1; i <= 5; i++)
            {
                await _service.CreateAsync(new CreateTaskItemDTO(
                    $"Task {i}", "Desc", Priority.Medium,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 1, null), CancellationToken.None);
            }

            // Act
            var result = await _service.CountAsync(null, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(5);
        }

        [Fact]
        public async Task CountAsync_WithPredicate_ReturnsFilteredCount()
        {
            // Arrange
            await _service.CreateAsync(new CreateTaskItemDTO(
                "High Priority", "Desc", Priority.High,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null), CancellationToken.None);
            await _service.CreateAsync(new CreateTaskItemDTO(
                "Low Priority", "Desc", Priority.Low,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), 1, null), CancellationToken.None);

            // Act
            var result = await _service.CountAsync(dto => dto.Priority == Priority.High, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(1);
        }

        #endregion
    }
}
