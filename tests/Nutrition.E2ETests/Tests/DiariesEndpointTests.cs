using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Nutrition.E2ETests.Fixtures;
using Nutrition.E2ETests.Helpers;

namespace Nutrition.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class DiariesEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private HttpClient _client = null!;

        public DiariesEndpointTests(CustomWebApplicationFactory factory)
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
            var response = await _client.GetAsync("/api/diaries");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            var body = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd") };

            var response = await _client.PostAsync("/api/diaries", body.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<int>();
            envelope!.Success.Should().BeTrue();
            envelope.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task GetById_WhenExists_ShouldReturnOk()
        {
            var body = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd") };
            var createResponse = await _client.PostAsync("/api/diaries", body.ToJsonContent());
            var createdId = await createResponse.DeserializeDataAsync<int>();

            var response = await _client.GetAsync($"/api/diaries/{createdId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_WhenNotExists_ShouldReturnNotFound()
        {
            var response = await _client.GetAsync("/api/diaries/99999");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Update_WhenExists_ShouldReturnOk()
        {
            var createBody = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd") };
            var createResponse = await _client.PostAsync("/api/diaries", createBody.ToJsonContent());
            var createdId = await createResponse.DeserializeDataAsync<int>();

            var updateBody = new { Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)).ToString("yyyy-MM-dd") };
            var response = await _client.PutAsync($"/api/diaries/{createdId}", updateBody.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Delete_WhenExists_ShouldReturnOk()
        {
            var createBody = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd") };
            var createResponse = await _client.PostAsync("/api/diaries", createBody.ToJsonContent());
            var createdId = await createResponse.DeserializeDataAsync<int>();

            var response = await _client.DeleteAsync($"/api/diaries/{createdId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
