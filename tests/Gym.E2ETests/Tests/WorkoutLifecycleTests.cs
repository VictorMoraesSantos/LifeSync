using FluentAssertions;
using Gym.Domain.Enums;
using Gym.E2ETests.Fixtures;
using Gym.E2ETests.Helpers;
using System.Net;

namespace Gym.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class WorkoutLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public WorkoutLifecycleTests(CustomWebApplicationFactory factory)
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

        private async Task<int> CreateExerciseAsync(string name)
        {
            var response = await _client.PostAsync("/api/Exercises", new
            {
                Name = name,
                Description = $"{name} description",
                MuscleGroup = (int)MuscleGroup.Chest,
                ExerciseType = (int)ExerciseType.Strength,
                EquipmentType = (int)EquipmentType.Barbell
            }.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            return await response.DeserializeDataAsync<int>();
        }

        private async Task<int> CreateRoutineAsync(string name)
        {
            var response = await _client.PostAsync("/api/Routines", new
            {
                Name = name,
                Description = $"{name} description"
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

        [Fact]
        public async Task FullWorkoutLifecycle_CreateExerciseRoutineAndSession()
        {
            // Step 1: Create exercises
            var exerciseId = await CreateExerciseAsync("Bench Press");
            exerciseId.Should().BeGreaterThan(0);

            // Step 2: Create a routine
            var routineId = await CreateRoutineAsync("Push Day");
            routineId.Should().BeGreaterThan(0);

            // Step 3: Create a training session
            var sessionId = await CreateTrainingSessionAsync(routineId);
            sessionId.Should().BeGreaterThan(0);

            // Step 4: Verify the session exists
            var getResponse = await _client.GetAsync($"/api/training-sessions/{sessionId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateAndDeleteExercise_ShouldWorkCorrectly()
        {
            // Create
            var exerciseId = await CreateExerciseAsync("Deadlift");
            exerciseId.Should().BeGreaterThan(0);

            // Verify exists
            var getResponse = await _client.GetAsync($"/api/Exercises/{exerciseId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Delete
            var deleteResponse = await _client.DeleteAsync($"/api/Exercises/{exerciseId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify deleted
            var getAfterDelete = await _client.GetAsync($"/api/Exercises/{exerciseId}");
            getAfterDelete.StatusCode.Should().Be(HttpStatusCode.OK);
            var deletedEnvelope = await getAfterDelete.DeserializeEnvelopeAsync<object>();
            deletedEnvelope!.Success.Should().BeFalse();
            deletedEnvelope.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task CreateAndUpdateExercise_ShouldPersistChanges()
        {
            // Create
            var exerciseId = await CreateExerciseAsync("Old Exercise");

            // Update
            var updateResponse = await _client.PutAsync($"/api/Exercises/{exerciseId}", new
            {
                Name = "Updated Exercise",
                Description = "Updated description",
                MuscleGroup = (int)MuscleGroup.Back,
                ExerciseType = (int)ExerciseType.Hypertrophy,
                EquipmentType = (int)EquipmentType.Cable
            }.ToJsonContent());

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateAndUpdateRoutine_ShouldPersistChanges()
        {
            // Create
            var routineId = await CreateRoutineAsync("Old Routine");

            // Update
            var updateResponse = await _client.PutAsync($"/api/Routines/{routineId}", new
            {
                Name = "Updated Routine",
                Description = "Updated description"
            }.ToJsonContent());

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateMultipleExercisesAndListAll_ShouldReturnAll()
        {
            // Create multiple exercises
            await CreateExerciseAsync("Exercise 1");
            await CreateExerciseAsync("Exercise 2");
            await CreateExerciseAsync("Exercise 3");

            // Get all
            var response = await _client.GetAsync("/api/Exercises");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
        }
    }
}
