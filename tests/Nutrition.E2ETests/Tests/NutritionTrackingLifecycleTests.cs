using FluentAssertions;
using Nutrition.E2ETests.Fixtures;
using Nutrition.E2ETests.Helpers;
using System.Net;

namespace Nutrition.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class NutritionTrackingLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private HttpClient _client = null!;

        public NutritionTrackingLifecycleTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            _client = _factory.CreateClient();
            await _factory.ResetDatabaseAsync();
        }

        public Task DisposeAsync()
        {
            _client?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task FullDiaryLifecycle_CreateReadUpdateDelete()
        {
            // Create
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var createBody = new { UserId = 1, Date = date.ToString("yyyy-MM-dd") };
            var createResponse = await _client.PostAsync("/api/diaries", createBody.ToJsonContent());
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createEnvelope = await createResponse.DeserializeEnvelopeAsync<int>();
            createEnvelope!.Success.Should().BeTrue();
            createEnvelope.StatusCode.Should().Be(201);
            var diaryId = createEnvelope.Data;
            diaryId.Should().BeGreaterThan(0);

            // Read
            var getResponse = await _client.GetAsync($"/api/diaries/{diaryId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Update
            var newDate = date.AddDays(1);
            var updateBody = new { Date = newDate.ToString("yyyy-MM-dd") };
            var updateResponse = await _client.PutAsync($"/api/diaries/{diaryId}", updateBody.ToJsonContent());
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Delete
            var deleteResponse = await _client.DeleteAsync($"/api/diaries/{diaryId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify deleted
            var verifyResponse = await _client.GetAsync($"/api/diaries/{diaryId}");
            verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var verifyEnvelope = await verifyResponse.DeserializeEnvelopeAsync<object>();
            verifyEnvelope!.Success.Should().BeFalse();
            verifyEnvelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task FullDailyProgressLifecycle_CreateReadUpdateDelete()
        {
            // Create
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var createBody = new
            {
                UserId = 1,
                Date = today.ToString("yyyy-MM-dd"),
                CaloriesConsumed = 500,
                LiquidsConsumedMl = 1000
            };
            var createResponse = await _client.PostAsync("/api/daily-progresses", createBody.ToJsonContent());
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createEnvelope = await createResponse.DeserializeEnvelopeAsync<int>();
            createEnvelope!.Success.Should().BeTrue();
            createEnvelope.StatusCode.Should().Be(201);
            var progressId = createEnvelope.Data;
            progressId.Should().BeGreaterThan(0);

            // Read
            var getResponse = await _client.GetAsync($"/api/daily-progresses/{progressId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Update
            var updateBody = new
            {
                CaloriesConsumed = 1200,
                LiquidsConsumedMl = 2500
            };
            var updateResponse = await _client.PutAsync($"/api/daily-progresses/{progressId}", updateBody.ToJsonContent());
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Delete
            var deleteResponse = await _client.DeleteAsync($"/api/daily-progresses/{progressId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify deleted
            var verifyResponse = await _client.GetAsync($"/api/daily-progresses/{progressId}");
            verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var verifyEnvelope = await verifyResponse.DeserializeEnvelopeAsync<object>();
            verifyEnvelope!.Success.Should().BeFalse();
            verifyEnvelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DiaryWithMeal_CreateDiaryThenAddMeal()
        {
            // Create diary
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var diaryBody = new { UserId = 1, Date = date.ToString("yyyy-MM-dd") };
            var diaryResponse = await _client.PostAsync("/api/diaries", diaryBody.ToJsonContent());
            diaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var diaryEnvelope = await diaryResponse.DeserializeEnvelopeAsync<int>();
            diaryEnvelope!.Success.Should().BeTrue();
            diaryEnvelope.StatusCode.Should().Be(201);
            var diaryId = diaryEnvelope.Data;

            // Create meal for that diary
            var mealBody = new { DiaryId = diaryId, Name = "Breakfast", Description = "Morning meal" };
            var mealResponse = await _client.PostAsync("/api/meals", mealBody.ToJsonContent());
            mealResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var mealEnvelope = await mealResponse.DeserializeEnvelopeAsync<object>();
            mealEnvelope!.Success.Should().BeTrue();
            mealEnvelope.StatusCode.Should().Be(201);

            // Verify diary still accessible
            var getResponse = await _client.GetAsync($"/api/diaries/{diaryId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DailyProgress_SetGoal_ShouldSucceed()
        {
            // Create daily progress
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var createBody = new
            {
                UserId = 5,
                Date = today.ToString("yyyy-MM-dd"),
                CaloriesConsumed = 0,
                LiquidsConsumedMl = 0
            };
            var createResponse = await _client.PostAsync("/api/daily-progresses", createBody.ToJsonContent());
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createEnvelope = await createResponse.DeserializeEnvelopeAsync<int>();
            createEnvelope!.Success.Should().BeTrue();
            createEnvelope.StatusCode.Should().Be(201);
            var progressId = createEnvelope.Data;

            // Set goal
            var goalBody = new { Goal = new { Calories = 2000, QuantityMl = 3000 } };
            var goalResponse = await _client.PostAsync($"/api/daily-progresses/{progressId}/set-goal", goalBody.ToJsonContent());
            goalResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify progress still accessible
            var getResponse = await _client.GetAsync($"/api/daily-progresses/{progressId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnUserSpecificData()
        {
            // Create diary for user 10
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var diaryBody = new { UserId = 10, Date = date.ToString("yyyy-MM-dd") };
            var diaryResponse = await _client.PostAsync("/api/diaries", diaryBody.ToJsonContent());
            diaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var diaryEnvelope = await diaryResponse.DeserializeEnvelopeAsync<int>();
            diaryEnvelope!.Success.Should().BeTrue();
            diaryEnvelope.StatusCode.Should().Be(201);

            // Get diaries by user
            var getResponse = await _client.GetAsync("/api/diaries/user/10");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
