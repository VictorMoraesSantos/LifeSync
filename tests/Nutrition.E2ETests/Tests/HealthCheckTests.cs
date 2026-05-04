using FluentAssertions;
using Nutrition.E2ETests.Fixtures;
using System.Net;

namespace Nutrition.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private HttpClient _client = null!;

        public HealthCheckTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public Task InitializeAsync()
        {
            _client = _factory.CreateClient();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _client?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task HealthCheck_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/health");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task HealthCheck_ShouldContainServiceName()
        {
            var response = await _client.GetAsync("/health");
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Nutrition");
        }
    }
}
