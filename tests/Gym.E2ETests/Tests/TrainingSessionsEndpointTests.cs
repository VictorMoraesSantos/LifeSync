using System.Net;
using Gym.Domain.Enums;
using Gym.E2ETests.Fixtures;
using Gym.E2ETests.Helpers;
using FluentAssertions;

namespace Gym.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class TrainingSessionsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TrainingSessionsEndpointTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        #region Helpers

        private async Task<int> CreateRoutineAsync()
        {
            var response = await _client.PostAsync("/api/Routines", new
            {
                Name = "Test Routine",
                Description = "Routine for training session tests"
            }.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            return await response.DeserializeDataAsync<int>();
        }

        private async Task<int> CreateTrainingSessionAsync(int routineId)
        {
            var response = await _client.PostAsync("/api/training-sessions", new
            {
                UserId = 1,
                RoutineId = routineId,
                StartTime = DateTime.UtcNow.AddHours(-1).ToString("o"),
                EndTime = DateTime.UtcNow.ToString("o")
            }.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            return await response.DeserializeDataAsync<int>();
        }

        #endregion

        #region POST Tests

        [Fact]
        public async Task Create_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();

            // Act
            var response = await _client.PostAsync("/api/training-sessions", new
            {
                UserId = 1,
                RoutineId = routineId,
                StartTime = DateTime.UtcNow.AddHours(-1).ToString("o"),
                EndTime = DateTime.UtcNow.ToString("o")
            }.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
        }

        #endregion

        #region GET Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnSession()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();
            var sessionId = await CreateTrainingSessionAsync(routineId);

            // Act
            var response = await _client.GetAsync($"/api/training-sessions/{sessionId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();
            await CreateTrainingSessionAsync(routineId);

            // Act
            var response = await _client.GetAsync("/api/training-sessions");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Search_WithoutFilters_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/training-sessions/search?Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public async Task Delete_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var routineId = await CreateRoutineAsync();
            var sessionId = await CreateTrainingSessionAsync(routineId);

            // Act
            var response = await _client.DeleteAsync($"/api/training-sessions/{sessionId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion
    }
}
