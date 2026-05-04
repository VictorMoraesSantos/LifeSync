using FluentAssertions;
using Nutrition.E2ETests.Fixtures;
using Nutrition.E2ETests.Helpers;
using System.Net;

namespace Nutrition.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class DailyProgressEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private HttpClient _client = null!;

        public DailyProgressEndpointTests(CustomWebApplicationFactory factory)
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
        public async Task GetAll_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/daily-progresses");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            var body = new
            {
                UserId = 1,
                Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
                CaloriesConsumed = 500,
                LiquidsConsumedMl = 1000
            };

            var response = await _client.PostAsync("/api/daily-progresses", body.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task GetById_WhenExists_ShouldReturnOk()
        {
            var body = new
            {
                UserId = 2,
                Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
                CaloriesConsumed = 200,
                LiquidsConsumedMl = 400
            };
            var createResponse = await _client.PostAsync("/api/daily-progresses", body.ToJsonContent());
            var createdId = await createResponse.DeserializeDataAsync<int>();

            var response = await _client.GetAsync($"/api/daily-progresses/{createdId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_WhenNotExists_ShouldReturnNotFound()
        {
            var response = await _client.GetAsync("/api/daily-progresses/99999");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Delete_WhenExists_ShouldReturnOk()
        {
            var body = new
            {
                UserId = 3,
                Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd"),
                CaloriesConsumed = 100,
                LiquidsConsumedMl = 200
            };
            var createResponse = await _client.PostAsync("/api/daily-progresses", body.ToJsonContent());
            var createdId = await createResponse.DeserializeDataAsync<int>();

            var response = await _client.DeleteAsync($"/api/daily-progresses/{createdId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
