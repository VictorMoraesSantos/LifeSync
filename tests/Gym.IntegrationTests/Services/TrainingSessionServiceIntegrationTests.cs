using FluentAssertions;
using Gym.Application.DTOs.TrainingSession;
using Gym.Infrastructure.Persistence.Repositories;
using Gym.Infrastructure.Services;
using Gym.IntegrationTests.Fixtures;
using Gym.IntegrationTests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gym.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class TrainingSessionServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;

        public TrainingSessionServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private TrainingSessionService CreateService()
        {
            var context = _fixture.CreateNewContext();
            var repo = new TrainingSessionRepository(context);
            return new TrainingSessionService(repo, NullLogger<TrainingSessionService>.Instance);
        }

        private async Task<int> CreateRoutineAsync()
        {
            var context = _fixture.CreateNewContext();
            var routine = TestDataFactory.CreateRoutine();
            await context.Routines.AddAsync(routine);
            await context.SaveChangesAsync();
            return routine.Id;
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateSession()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();
            var service = CreateService();
            var dto = new CreateTrainingSessionDTO(1, routineId, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);

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
        public async Task GetByIdAsync_WithExistingSession_ShouldReturnDTO()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateTrainingSessionDTO(1, routineId, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow));

            // Act
            var service2 = CreateService();
            var result = await service2.GetByIdAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.RoutineId.Should().Be(routineId);
            result.Value.UserId.Should().Be(1);
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

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithExistingSessions_ShouldReturnAll()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();
            var service = CreateService();
            await service.CreateAsync(new CreateTrainingSessionDTO(1, routineId, DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1)));
            await service.CreateAsync(new CreateTrainingSessionDTO(2, routineId, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow));

            // Act
            var service2 = CreateService();
            var result = await service2.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        #endregion

        #region GetByUserIdAsync Tests

        [Fact]
        public async Task GetByUserIdAsync_WithExistingSessions_ShouldReturnUserSessions()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();
            var service = CreateService();
            await service.CreateAsync(new CreateTrainingSessionDTO(42, routineId, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow));
            await service.CreateAsync(new CreateTrainingSessionDTO(99, routineId, DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1)));

            // Act
            var service2 = CreateService();
            var result = await service2.GetByUserIdAsync(42, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingSession_ShouldSucceed()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateTrainingSessionDTO(1, routineId, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow));

            // Act
            var service2 = CreateService();
            var result = await service2.DeleteAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Act
            var service = CreateService();
            var result = await service.DeleteAsync(99999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion
    }
}
