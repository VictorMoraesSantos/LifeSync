using Financial.Domain.Enums;
using Financial.E2ETests.Fixtures;
using Financial.E2ETests.Helpers;
using FluentAssertions;
using System.Net;

namespace Financial.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class RecurrenceScheduleLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public RecurrenceScheduleLifecycleTests(CustomWebApplicationFactory factory)
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
                Name = "Lifecycle Category",
                Description = "Category for lifecycle tests"
            }.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            return await response.DeserializeDataAsync<int>();
        }

        private async Task<int> CreateTransactionAsync(int categoryId, bool isRecurring = true)
        {
            var response = await _client.PostAsync("/api/Transaction", new
            {
                CategoryId = categoryId,
                PaymentMethod = (int)PaymentMethod.Pix,
                TransactionType = (int)TransactionType.Expense,
                Amount = new { Amount = 5000, Currency = (int)Financial.Domain.Enums.Currency.BRL },
                Description = "Monthly rent",
                TransactionDate = DateTime.UtcNow.ToString("o"),
                IsRecurring = isRecurring
            }.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            return await response.DeserializeDataAsync<int>();
        }

        #endregion

        [Fact]
        public async Task DeactivateAndDelete_NonExistingSchedule_ShouldReturnNotFound()
        {
            // Act - Deactivate non-existing
            var deactivateResponse = await _client.PatchAsync("/api/RecurrenceSchedule/99999/deactivate", null);
            deactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deactivateEnvelope = await deactivateResponse.DeserializeEnvelopeAsync<object>();
            deactivateEnvelope!.Success.Should().BeFalse();
            deactivateEnvelope.StatusCode.Should().Be(404);

            // Act - Delete non-existing
            var deleteResponse = await _client.DeleteAsync("/api/RecurrenceSchedule/99999");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deleteEnvelope = await deleteResponse.DeserializeEnvelopeAsync<object>();
            deleteEnvelope!.Success.Should().BeFalse();
            deleteEnvelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Search_WithPagination_ShouldReturnPaginatedResults()
        {
            // Act
            var response = await _client.GetAsync("/api/RecurrenceSchedule/search?Page=1&PageSize=5");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Search_WithActiveFilter_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/RecurrenceSchedule/search?IsActive=true&Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(1)] // Daily
        [InlineData(2)] // Weekly
        [InlineData(3)] // Monthly
        [InlineData(4)] // Yearly
        public async Task Search_WithFrequencyFilter_ShouldReturnOk(int frequency)
        {
            // Act
            var response = await _client.GetAsync($"/api/RecurrenceSchedule/search?Frequency={frequency}&Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
