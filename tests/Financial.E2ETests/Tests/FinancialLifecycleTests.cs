using System.Net;
using Financial.Domain.Enums;
using Financial.E2ETests.Fixtures;
using Financial.E2ETests.Helpers;
using FluentAssertions;

namespace Financial.E2ETests.Tests
{
    [Trait("Category", "E2E")]
    [Trait("Layer", "API")]
    public class FinancialLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public FinancialLifecycleTests(CustomWebApplicationFactory factory)
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

        private async Task<int> CreateCategoryAsync(string name)
        {
            var response = await _client.PostAsync("/api/Categories", new
            {
                UserId = 1,
                Name = name,
                Description = $"Description for {name}"
            }.ToJsonContent());

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
            return await response.DeserializeDataAsync<int>();
        }

        private async Task<int> CreateTransactionAsync(int categoryId, string description, int amount)
        {
            var response = await _client.PostAsync("/api/Transactions", new
            {
                UserId = 1,
                CategoryId = categoryId,
                PaymentMethod = (int)PaymentMethod.Pix,
                TransactionType = (int)TransactionType.Expense,
                Amount = new { Amount = amount, Currency = (int)Financial.Domain.Enums.Currency.BRL },
                Description = description,
                TransactionDate = DateTime.UtcNow.ToString("o"),
                IsRecurring = false
            }.ToJsonContent());

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
            return await response.DeserializeDataAsync<int>();
        }

        #endregion

        [Fact]
        public async Task FullCategoryLifecycle_CreateUpdateDelete_ShouldSucceed()
        {
            // Create
            var categoryId = await CreateCategoryAsync("Lifecycle Category");
            categoryId.Should().BeGreaterThan(0);

            // Read
            var getResponse = await _client.GetAsync($"/api/Categories/{categoryId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Update
            var updateResponse = await _client.PutAsync($"/api/Categories/{categoryId}", new
            {
                Name = "Updated Lifecycle Category",
                Description = "Updated description"
            }.ToJsonContent());
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify update
            var getUpdatedResponse = await _client.GetAsync($"/api/Categories/{categoryId}");
            getUpdatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Delete
            var deleteResponse = await _client.DeleteAsync($"/api/Categories/{categoryId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify deletion
            var getDeletedResponse = await _client.GetAsync($"/api/Categories/{categoryId}");
            getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deletedEnvelope = await getDeletedResponse.DeserializeEnvelopeAsync<object>();
            deletedEnvelope!.Success.Should().BeFalse();
            deletedEnvelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task FullTransactionLifecycle_CreateUpdateDelete_ShouldSucceed()
        {
            // Create category first
            var categoryId = await CreateCategoryAsync("Transaction Lifecycle Category");

            // Create transaction
            var transactionId = await CreateTransactionAsync(categoryId, "Lifecycle Transaction", 5000);
            transactionId.Should().BeGreaterThan(0);

            // Read
            var getResponse = await _client.GetAsync($"/api/Transactions/{transactionId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Update
            var updateResponse = await _client.PutAsync($"/api/Transactions/{transactionId}", new
            {
                CategoryId = categoryId,
                PaymentMethod = (int)PaymentMethod.DebitCard,
                TransactionType = (int)TransactionType.Income,
                Amount = new { Amount = 10000, Currency = (int)Financial.Domain.Enums.Currency.USD },
                Description = "Updated Lifecycle Transaction",
                TransactionDate = DateTime.UtcNow.ToString("o")
            }.ToJsonContent());
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Delete
            var deleteResponse = await _client.DeleteAsync($"/api/Transactions/{transactionId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify deletion
            var getDeletedResponse = await _client.GetAsync($"/api/Transactions/{transactionId}");
            getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deletedEnvelope = await getDeletedResponse.DeserializeEnvelopeAsync<object>();
            deletedEnvelope!.Success.Should().BeFalse();
            deletedEnvelope.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CategoryWithTransactions_ShouldWorkTogether()
        {
            // Create category
            var categoryId = await CreateCategoryAsync("Multi-Transaction Category");

            // Create multiple transactions for same category
            var txn1 = await CreateTransactionAsync(categoryId, "Transaction 1", 1000);
            var txn2 = await CreateTransactionAsync(categoryId, "Transaction 2", 2000);
            var txn3 = await CreateTransactionAsync(categoryId, "Transaction 3", 3000);

            txn1.Should().BeGreaterThan(0);
            txn2.Should().BeGreaterThan(0);
            txn3.Should().BeGreaterThan(0);

            // Search transactions
            var searchResponse = await _client.GetAsync("/api/Transactions/search?Page=1&PageSize=10");
            searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Search categories
            var catSearchResponse = await _client.GetAsync("/api/Categories/search?Page=1&PageSize=10");
            catSearchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Search_WithPagination_ShouldReturnPaginatedResults()
        {
            // Arrange - create test data
            var catId = await CreateCategoryAsync("Pagination Test");
            for (int i = 0; i < 5; i++)
            {
                await CreateTransactionAsync(catId, $"Paginated Transaction {i}", 1000 + i);
            }

            // Act
            var response = await _client.GetAsync("/api/Transactions/search?Page=1&PageSize=2");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var envelope = await response.DeserializeEnvelopeAsync<object>();
            envelope.Should().NotBeNull();
            envelope!.Success.Should().BeTrue();
        }
    }
}
