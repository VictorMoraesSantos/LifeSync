using System.Net;
using Gym.E2ETests.Fixtures;
using Gym.E2ETests.Helpers;
using FluentAssertions;

namespace Gym.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class RoutinesEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public RoutinesEndpointTests(CustomWebApplicationFactory factory)
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

        private async Task<int> CreateRoutineAsync(string name = "Push Day")
        {
            var response = await _client.PostAsync("/api/Routines", new
            {
                Name = name,
                Description = "Test routine description"
            }.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var id = await response.DeserializeDataAsync<int>();
            return id;
        }

        #endregion

        #region POST Tests

        [Fact]
        public async Task Create_WithValidData_ShouldReturnOk()
        {
            // Act
            var response = await _client.PostAsync("/api/Routines", new
            {
                Name = "Leg Day",
                Description = "Quadriceps, hamstrings, calves"
            }.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
            envelope.Data.Should().BeGreaterThan(0);
        }

        #endregion

        #region GET Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnRoutine()
        {
            // Arrange
            var id = await CreateRoutineAsync();

            // Act
            var response = await _client.GetAsync($"/api/Routines/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            await CreateRoutineAsync("Routine A");

            // Act
            var response = await _client.GetAsync("/api/Routines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Search_WithoutFilters_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/Routines/search?Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public async Task Delete_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var id = await CreateRoutineAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/Routines/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion
    }
}
