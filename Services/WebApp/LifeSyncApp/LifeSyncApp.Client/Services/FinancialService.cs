using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Services.Http;
using System.Net.Http.Json;

namespace LifeSyncApp.Client.Services
{
    // Financial Models
    public class Transaction
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = "Expense"; // Income or Expense
        public string Category { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "Cash";
        public bool IsRecurring { get; set; }
    }

    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = "#000000";
        public string Type { get; set; } = "Expense";
    }

    // Lightweight filter models that match API query parameter names
    public class TransactionFilter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionType { get; set; }
        public int? AmountEquals { get; set; }
        public int? AmountGreaterThan { get; set; }
        public int? AmountLessThan { get; set; }
        public string? CurrencyEquals { get; set; }
        public string? DescriptionContains { get; set; }
        public DateOnly? TransactionDate { get; set; }
        public DateOnly? TransactionDateFrom { get; set; }
        public DateOnly? TransactionDateTo { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class CategoryFilter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? NameContains { get; set; }
        public string? DescriptionContains { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    // Financial Service
    public interface IFinancialService
    {
        Task<ApiResponse<List<Transaction>>> GetTransactionsAsync();
        Task<ApiResponse<Transaction>> CreateTransactionAsync(Transaction transaction);
        Task<ApiResponse<Transaction>> UpdateTransactionAsync(Guid id, Transaction transaction);
        Task<ApiResponse<object>> DeleteTransactionAsync(Guid id);
        Task<ApiResponse<List<Category>>> GetCategoriesAsync();

        // New search endpoints
        Task<ApiResponse<List<Transaction>>> SearchTransactionsAsync(TransactionFilter filter);
        Task<ApiResponse<List<Category>>> SearchCategoriesAsync(CategoryFilter filter);
        Task<ApiResponse<List<Transaction>>> GetTransactionsByUserAsync(int userId);
        Task<ApiResponse<List<Category>>> GetCategoriesByUserAsync(int userId);
    }

    public class FinancialService : IFinancialService
    {
        private readonly IApiClient _apiClient;

        public FinancialService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<Transaction>>> GetTransactionsAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<Transaction>>>("financial-service/api/transactions");
                return res ?? new ApiResponse<List<Transaction>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Transaction>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<Transaction>> CreateTransactionAsync(Transaction transaction)
        {
            try
            {
                var res = await _apiClient.PostAsync<Transaction, ApiResponse<Transaction>>("financial-service/api/transactions", transaction);
                return res ?? new ApiResponse<Transaction> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Transaction> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<Transaction>> UpdateTransactionAsync(Guid id, Transaction transaction)
        {
            try
            {
                var res = await _apiClient.PutAsync<Transaction, ApiResponse<Transaction>>($"financial-service/api/transactions/{id}", transaction);
                return res ?? new ApiResponse<Transaction> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Transaction> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteTransactionAsync(Guid id)
        {
            try
            {
                await _apiClient.DeleteAsync($"financial-service/api/transactions/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Category>>> GetCategoriesAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<Category>>>("financial-service/api/categories");
                return res ?? new ApiResponse<List<Category>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Category>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Transaction>>> SearchTransactionsAsync(TransactionFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<Transaction>>>($"financial-service/api/transactions/search{query}");
                return res ?? new ApiResponse<List<Transaction>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Transaction>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Category>>> SearchCategoriesAsync(CategoryFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<Category>>>($"financial-service/api/categories/search{query}");
                return res ?? new ApiResponse<List<Category>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Category>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Transaction>>> GetTransactionsByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<Transaction>>>($"financial-service/api/transactions/user/{userId}");
                return res ?? new ApiResponse<List<Transaction>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Transaction>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Category>>> GetCategoriesByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<Category>>>($"financial-service/api/categories/user/{userId}");
                return res ?? new ApiResponse<List<Category>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Category>> { Success = false, Errors = new[] { ex.Message } };
            }
        }
    }
}
