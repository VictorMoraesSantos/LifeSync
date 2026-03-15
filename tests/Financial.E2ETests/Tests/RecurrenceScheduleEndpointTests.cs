using System.Net;
using Financial.Domain.Enums;
using Financial.E2ETests.Fixtures;
using Financial.E2ETests.Helpers;
using FluentAssertions;

namespace Financial.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class RecurrenceScheduleEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public RecurrenceScheduleEndpointTests(CustomWebApplicationFactory factory)
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

        private async Task<int> CreateCategoryAsync()
        {
            var response = await _client.PostAsync("/api/Category", new
            {
                Name = "Test Category",
                Description = "Test category description"
            }.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var id = await response.DeserializeDataAsync<int>();
            return id;
        }

        private async Task<int> CreateTransactionAsync(int? categoryId = null, bool isRecurring = true)
        {
            var catId = categoryId ?? await CreateCategoryAsync();

            var response = await _client.PostAsync("/api/Transaction", new
            {
                CategoryId = catId,
                PaymentMethod = (int)PaymentMethod.Pix,
                TransactionType = (int)TransactionType.Expense,
                Amount = new { Amount = 1000, Currency = (int)Financial.Domain.Enums.Currency.BRL },
                Description = "Recurring transaction test",
                TransactionDate = DateTime.UtcNow.ToString("o"),
                IsRecurring = isRecurring
            }.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var id = await response.DeserializeDataAsync<int>();
            return id;
        }

        #endregion

        #region GET Tests

        [Fact]
        public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/RecurrenceSchedule/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnOkOrNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/RecurrenceSchedule/user/1");

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Search_WithoutFilters_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/RecurrenceSchedule/search?Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Search_WithFrequencyFilter_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/RecurrenceSchedule/search?Frequency=1&Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region PATCH Tests

        [Fact]
        public async Task Deactivate_WithNonExistingId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.PatchAsync("/api/RecurrenceSchedule/99999/deactivate", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public async Task Delete_WithNonExistingId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/RecurrenceSchedule/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        #endregion
    }
}
