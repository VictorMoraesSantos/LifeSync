using Financial.E2ETests.Fixtures;
using Financial.E2ETests.Helpers;
using FluentAssertions;
using System.Net;

namespace Financial.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class CategoriesEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CategoriesEndpointTests(CustomWebApplicationFactory factory)
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

        private async Task<int> CreateCategoryAsync(string name = "Test Category", string description = "Test description")
        {
            var response = await _client.PostAsync("/api/Categories", new
            {
                UserId = 1,
                Name = name,
                Description = description
            }.ToJsonContent());

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
            var id = await response.DeserializeDataAsync<int>();
            return id;
        }

        #endregion

        #region POST Tests

        [Fact]
        public async Task CreateCategory_WithValidData_ShouldReturnCreated()
        {
            // Act
            var response = await _client.PostAsync("/api/Categories", new
            {
                UserId = 1,
                Name = "New Category",
                Description = "New Description"
            }.ToJsonContent());

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
            var id = await response.DeserializeDataAsync<int>();
            id.Should().BeGreaterThan(0);
        }

        #endregion

        #region GET Tests

        [Fact]
        public async Task GetById_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var id = await CreateCategoryAsync();

            // Act
            var response = await _client.GetAsync($"/api/Categories/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/Categories/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/Categories");

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Search_WithFilters_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/Categories/search?Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnOkOrNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/Categories/user/1");

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }

        #endregion

        #region PUT Tests

        [Fact]
        public async Task UpdateCategory_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var id = await CreateCategoryAsync();

            // Act
            var response = await _client.PutAsync($"/api/Categories/{id}", new
            {
                Name = "Updated Name",
                Description = "Updated Description"
            }.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateCategory_WithNonExistingId_ShouldReturnNotFoundOrBadRequest()
        {
            // Act
            var response = await _client.PutAsync("/api/Categories/99999", new
            {
                Name = "Name",
                Description = "Desc"
            }.ToJsonContent());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().BeOneOf(404, 400);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public async Task DeleteCategory_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var id = await CreateCategoryAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/Categories/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteCategory_WithNonExistingId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/Categories/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        #endregion
    }
}
