using FluentAssertions;
using Gym.Domain.Enums;
using Gym.E2ETests.Fixtures;
using Gym.E2ETests.Helpers;
using System.Net;

namespace Gym.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class ExercisesEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ExercisesEndpointTests(CustomWebApplicationFactory factory)
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

        private async Task<int> CreateExerciseAsync(string name = "Bench Press")
        {
            var response = await _client.PostAsync("/api/Exercises", new
            {
                Name = name,
                Description = "Test exercise description",
                MuscleGroup = (int)MuscleGroup.Chest,
                ExerciseType = (int)ExerciseType.Strength,
                EquipmentType = (int)EquipmentType.Barbell
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
            var response = await _client.PostAsync("/api/Exercises", new
            {
                Name = "Squat",
                Description = "Barbell back squat",
                MuscleGroup = (int)MuscleGroup.Quadriceps,
                ExerciseType = (int)ExerciseType.Strength,
                EquipmentType = (int)EquipmentType.Barbell
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
        public async Task GetById_WithExistingId_ShouldReturnExercise()
        {
            // Arrange
            var id = await CreateExerciseAsync();

            // Act
            var response = await _client.GetAsync($"/api/Exercises/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/Exercises/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            await CreateExerciseAsync("Exercise A");
            await CreateExerciseAsync("Exercise B");

            // Act
            var response = await _client.GetAsync("/api/Exercises");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Search_WithoutFilters_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/Exercises/search?Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public async Task Delete_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var id = await CreateExerciseAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/Exercises/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion
    }
}
