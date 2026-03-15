using System.Net;
using Gym.E2ETests.Fixtures;
using FluentAssertions;

namespace Gym.E2ETests.Tests
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
        public async Task HealthCheck_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
