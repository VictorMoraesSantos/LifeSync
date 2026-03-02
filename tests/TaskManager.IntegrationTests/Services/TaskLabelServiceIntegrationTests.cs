using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Persistence.Repositories;
using TaskManager.Infrastructure.Services;
using TaskManager.IntegrationTests.Fixtures;

namespace TaskManager.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class TaskLabelServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly TaskLabelService _service;
        private readonly TaskLabelRepository _repository;

        public TaskLabelServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TaskLabelRepository(_fixture.DbContext);
            _service = new TaskLabelService(
                _repository,
                NullLogger<TaskLabelService>.Instance);
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
            var dto = new CreateTaskLabelDTO("Work", LabelColor.Blue, 1);

            // Act
            var result = await _service.CreateAsync(dto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
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
            var dtos = new List<CreateTaskLabelDTO>
            {
                new("Label 1", LabelColor.Red, 1),
                new("Label 2", LabelColor.Blue, 1),
                new("Label 3", LabelColor.Green, 1)
            };

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
            var result = await _service.CreateRangeAsync(
                Enumerable.Empty<CreateTaskLabelDTO>(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsSuccessWithDTO()
        {
            // Arrange
            var createResult = await _service.CreateAsync(
                new CreateTaskLabelDTO("Find Me", LabelColor.Purple, 1), CancellationToken.None);

            // Act
            var result = await _service.GetByIdAsync(createResult.Value, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value!.Name.Should().Be("Find Me");
            result.Value.LabelColor.Should().Be(LabelColor.Purple);
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
        public async Task GetAllAsync_WithMultipleLabels_ReturnsAllDTOs()
        {
            // Arrange
            await _service.CreateAsync(new CreateTaskLabelDTO("Label 1", LabelColor.Red, 1), CancellationToken.None);
            await _service.CreateAsync(new CreateTaskLabelDTO("Label 2", LabelColor.Blue, 1), CancellationToken.None);

            // Act
            var result = await _service.GetAllAsync(CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_WithNoLabels_ReturnsEmptyList()
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
        public async Task GetByFilterAsync_WithColorFilter_ReturnsFilteredResults()
        {
            // Arrange
            await _service.CreateAsync(new CreateTaskLabelDTO("Red Label", LabelColor.Red, 1), CancellationToken.None);
            await _service.CreateAsync(new CreateTaskLabelDTO("Blue Label", LabelColor.Blue, 1), CancellationToken.None);
            await _service.CreateAsync(new CreateTaskLabelDTO("Another Red", LabelColor.Red, 1), CancellationToken.None);

            var filter = new TaskLabelFilterDTO(
                null, 1, null, null, LabelColor.Red,
                null, null, null, null, null, null, null);

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByFilterAsync_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 8; i++)
            {
                await _service.CreateAsync(new CreateTaskLabelDTO(
                    $"Label {i}", LabelColor.Blue, 1), CancellationToken.None);
            }

            var filter = new TaskLabelFilterDTO(
                null, 1, null, null, null,
                null, null, null, null, null, 1, 3);

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(3);
            result.Value.Pagination.TotalItems.Should().Be(8);
        }

        [Fact]
        public async Task GetByFilterAsync_WithNameContains_ReturnsMatchingLabels()
        {
            // Arrange
            await _service.CreateAsync(new CreateTaskLabelDTO("Work Project", LabelColor.Red, 1), CancellationToken.None);
            await _service.CreateAsync(new CreateTaskLabelDTO("Personal", LabelColor.Blue, 1), CancellationToken.None);
            await _service.CreateAsync(new CreateTaskLabelDTO("Work Home", LabelColor.Green, 1), CancellationToken.None);

            var filter = new TaskLabelFilterDTO(
                null, null, null, "Work", null,
                null, null, null, null, null, null, null);

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(2);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WithValidDTO_ReturnsSuccess()
        {
            // Arrange
            var createResult = await _service.CreateAsync(
                new CreateTaskLabelDTO("Original", LabelColor.Red, 1), CancellationToken.None);

            var updateDto = new UpdateTaskLabelDTO(createResult.Value, "Updated Name", LabelColor.Blue);

            // Act
            var result = await _service.UpdateAsync(updateDto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var getResult = await _service.GetByIdAsync(createResult.Value, CancellationToken.None);
            getResult.Value!.Name.Should().Be("Updated Name");
            getResult.Value.LabelColor.Should().Be(LabelColor.Blue);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingId_ReturnsFailure()
        {
            // Arrange
            var updateDto = new UpdateTaskLabelDTO(99999, "Name", LabelColor.Red);

            // Act
            var result = await _service.UpdateAsync(updateDto, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_WithExistingId_ReturnsSuccessAndRemovesEntity()
        {
            // Arrange
            var createResult = await _service.CreateAsync(
                new CreateTaskLabelDTO("Delete Me", LabelColor.Red, 1), CancellationToken.None);

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
                var createResult = await _service.CreateAsync(new CreateTaskLabelDTO(
                    $"Label {i}", LabelColor.Blue, 1), CancellationToken.None);
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
            var createResult = await _service.CreateAsync(
                new CreateTaskLabelDTO("Label", LabelColor.Red, 1), CancellationToken.None);

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
            var result = await _service.DeleteRangeAsync(
                Enumerable.Empty<int>(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetPagedAsync

        [Fact]
        public async Task GetPagedAsync_WithValidParameters_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 7; i++)
            {
                await _service.CreateAsync(new CreateTaskLabelDTO(
                    $"Label {i}", LabelColor.Blue, 1), CancellationToken.None);
            }

            // Act
            var result = await _service.GetPagedAsync(1, 3, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(3);
            result.Value.TotalCount.Should().Be(7);
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
        public async Task CountAsync_WithLabels_ReturnsCorrectCount()
        {
            // Arrange
            for (int i = 1; i <= 4; i++)
            {
                await _service.CreateAsync(new CreateTaskLabelDTO(
                    $"Label {i}", LabelColor.Blue, 1), CancellationToken.None);
            }

            // Act
            var result = await _service.CountAsync(null, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(4);
        }

        [Fact]
        public async Task CountAsync_WithPredicate_ReturnsFilteredCount()
        {
            // Arrange
            await _service.CreateAsync(new CreateTaskLabelDTO("Red Label", LabelColor.Red, 1), CancellationToken.None);
            await _service.CreateAsync(new CreateTaskLabelDTO("Blue Label", LabelColor.Blue, 1), CancellationToken.None);
            await _service.CreateAsync(new CreateTaskLabelDTO("Another Red", LabelColor.Red, 1), CancellationToken.None);

            // Act
            var result = await _service.CountAsync(
                dto => dto.LabelColor == LabelColor.Red, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(2);
        }

        #endregion
    }
}
