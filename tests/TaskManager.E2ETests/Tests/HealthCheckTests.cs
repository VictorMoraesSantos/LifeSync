using FluentAssertions;
using System.Net;
using System.Text.Json;
using TaskManager.E2ETests.Fixtures;

namespace TaskManager.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HealthCheckTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GET_Health_ReturnsOkWithServiceInfo()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var root = json.RootElement;

            root.GetProperty("status").GetString().Should().Be("healthy");
            root.GetProperty("service").GetString().Should().Be("TaskManager");
        }
    }
}
