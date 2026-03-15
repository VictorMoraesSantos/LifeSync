using Gym.Application.DTOs.Routine;
using Gym.Infrastructure.Persistence.Repositories;
using Gym.Infrastructure.Services;
using Gym.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gym.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class RoutineServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;

        public RoutineServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private RoutineService CreateService()
        {
            var context = _fixture.CreateNewContext();
            var repo = new RoutineRepository(context);
            return new RoutineService(repo, NullLogger<RoutineService>.Instance);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateRoutine()
        {
            // Arrange
            var service = CreateService();
            var dto = new CreateRoutineDTO("Push Day", "Chest, shoulders, triceps");

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateAsync_WithNullDto_ShouldReturnFailure()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.CreateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingRoutine_ShouldReturnDTO()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(new CreateRoutineDTO("Pull Day", "Back and biceps"));

            // Act
            var service2 = CreateService();
            var result = await service2.GetByIdAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("Pull Day");
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Act
            var service = CreateService();
            var result = await service.GetByIdAsync(99999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateRoutine()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(new CreateRoutineDTO("Old Routine", "Old Desc"));

            var updateDto = new UpdateRoutineDTO(createResult.Value, "Updated Routine", "Updated Desc");

            // Act
            var service2 = CreateService();
            var result = await service2.UpdateAsync(updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var service3 = CreateService();
            var updated = await service3.GetByIdAsync(createResult.Value);
            updated.Value!.Name.Should().Be("Updated Routine");
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingRoutine_ShouldSucceed()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(new CreateRoutineDTO("To Delete", "Will be deleted"));

            // Act
            var service2 = CreateService();
            var result = await service2.DeleteAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var service3 = CreateService();
            var getResult = await service3.GetByIdAsync(createResult.Value);
            getResult.IsSuccess.Should().BeFalse();
        }

        #endregion
    }
}
