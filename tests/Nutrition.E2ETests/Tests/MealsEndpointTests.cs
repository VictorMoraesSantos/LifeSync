using System.Net;
using FluentAssertions;
using Nutrition.E2ETests.Fixtures;
using Nutrition.E2ETests.Helpers;

namespace Nutrition.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class MealsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private HttpClient _client = null!;

        public MealsEndpointTests(CustomWebApplicationFactory factory)
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

        private async Task<int> CreateDiaryAsync()
        {
            var body = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd") };
            var response = await _client.PostAsync("/api/diaries", body.ToJsonContent());
            return await response.DeserializeDataAsync<int>();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/meals");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            var diaryId = await CreateDiaryAsync();
            var body = new { DiaryId = diaryId, Name = "Breakfast", Description = "Morning meal" };

            var response = await _client.PostAsync("/api/meals", body.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Update_WhenExists_ShouldReturnOk()
        {
            var diaryId = await CreateDiaryAsync();
            var createBody = new { DiaryId = diaryId, Name = "OldName", Description = "OldDesc" };
            var createResponse = await _client.PostAsync("/api/meals", createBody.ToJsonContent());

            // Get the meal ID - need to find created meal
            var mealsResponse = await _client.GetAsync($"/api/meals/diary/{diaryId}");
            mealsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Try updating first meal available
            var allMeals = await _client.GetAsync("/api/meals");
            var envelope = await allMeals.DeserializeEnvelopeAsync<List<MealDto>>();
            if (envelope?.Data != null && envelope.Data.Count > 0)
            {
                var mealId = envelope.Data[0].Id;
                var updateBody = new { Name = "NewName", Description = "NewDesc" };
                var response = await _client.PutAsync($"/api/meals/{mealId}", updateBody.ToJsonContent());
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task GetById_WhenNotExists_ShouldReturnNotFound()
        {
            var response = await _client.GetAsync("/api/meals/99999");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Delete_WhenNotExists_ShouldReturnNotFound()
        {
            var response = await _client.DeleteAsync("/api/meals/99999");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        private record MealDto(int Id, string Name, string Description);
    }
}
