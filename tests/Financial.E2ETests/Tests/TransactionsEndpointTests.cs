using System.Net;
using Financial.Domain.Enums;
using Financial.E2ETests.Fixtures;
using Financial.E2ETests.Helpers;
using FluentAssertions;

namespace Financial.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class TransactionsEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TransactionsEndpointTests(CustomWebApplicationFactory factory)
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
            var response = await _client.PostAsync("/api/Categories", new
            {
                UserId = 1,
                Name = "Test Category",
                Description = "Test category description"
            }.ToJsonContent());

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
            return await response.DeserializeDataAsync<int>();
        }

        private async Task<int> CreateTransactionAsync(int? categoryId = null)
        {
            var catId = categoryId ?? await CreateCategoryAsync();

            var response = await _client.PostAsync("/api/Transactions", new
            {
                UserId = 1,
                CategoryId = catId,
                PaymentMethod = (int)PaymentMethod.Pix,
                TransactionType = (int)TransactionType.Expense,
                Amount = new { Amount = 1000, Currency = (int)Financial.Domain.Enums.Currency.BRL },
                Description = "Test transaction",
                TransactionDate = DateTime.UtcNow.ToString("o"),
                IsRecurring = false
            }.ToJsonContent());

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
            return await response.DeserializeDataAsync<int>();
        }

        #endregion

        #region POST Tests

        [Fact]
        public async Task CreateTransaction_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var catId = await CreateCategoryAsync();

            // Act
            var response = await _client.PostAsync("/api/Transactions", new
            {
                UserId = 1,
                CategoryId = catId,
                PaymentMethod = (int)PaymentMethod.CreditCard,
                TransactionType = (int)TransactionType.Expense,
                Amount = new { Amount = 5000, Currency = (int)Financial.Domain.Enums.Currency.BRL },
                Description = "New transaction",
                TransactionDate = DateTime.UtcNow.ToString("o"),
                IsRecurring = false
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
            var id = await CreateTransactionAsync();

            // Act
            var response = await _client.GetAsync($"/api/Transactions/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/Transactions/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Search_WithFilters_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/Transactions/search?Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Search_WithPaymentMethodFilter_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync($"/api/Transactions/search?PaymentMethod={(int)PaymentMethod.Pix}&Page=1&PageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnOkOrNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/Transactions/user/1");

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }

        #endregion

        #region PUT Tests

        [Fact]
        public async Task UpdateTransaction_WithNonExistingId_ShouldReturnNotFoundOrBadRequest()
        {
            // Arrange
            var catId = await CreateCategoryAsync();

            // Act
            var response = await _client.PutAsync("/api/Transactions/99999", new
            {
                CategoryId = catId,
                PaymentMethod = (int)PaymentMethod.Cash,
                TransactionType = (int)TransactionType.Income,
                Amount = new { Amount = 2000, Currency = (int)Financial.Domain.Enums.Currency.USD },
                Description = "Updated",
                TransactionDate = DateTime.UtcNow.ToString("o")
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
        public async Task DeleteTransaction_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var id = await CreateTransactionAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/Transactions/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteTransaction_WithNonExistingId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/Transactions/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope!.Success.Should().BeFalse();
            envelope.StatusCode.Should().Be(404);
        }

        #endregion
    }
}
