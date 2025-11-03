using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.Financial;
using System.Net.Http.Json;

namespace LifeSyncApp.Client.Services
{
    public class FinancialService : IFinancialService
    {
        private readonly HttpClient _httpClient;

        public FinancialService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResult<List<TransactionDto>>> GetTransactionsAsync(int? userId = null)
        {
            var url = userId.HasValue
                ? $"/financial-service/api/transactions/user/{userId.Value}"
                : "/financial-service/api/transactions";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<TransactionDto>>>();
            return result ?? new HttpResult<List<TransactionDto>> { Success = false };
        }

        public async Task<HttpResult<TransactionDto>> GetTransactionByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/financial-service/api/transactions/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<TransactionDto>>();
            return result ?? new HttpResult<TransactionDto> { Success = false };
        }

        public async Task<HttpResult<TransactionDto>> CreateTransactionAsync(CreateTransactionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/financial-service/api/transactions", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<TransactionDto>>();
            return result ?? new HttpResult<TransactionDto> { Success = false };
        }

        public async Task<HttpResult> UpdateTransactionAsync(int id, UpdateTransactionRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/financial-service/api/transactions/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteTransactionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/financial-service/api/transactions/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult<List<CategoryDto>>> GetCategoriesAsync(int? userId = null)
        {
            var url = userId.HasValue
                ? $"/financial-service/api/categories/user/{userId.Value}"
                : "/financial-service/api/categories";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<CategoryDto>>>();
            return result ?? new HttpResult<List<CategoryDto>> { Success = false };
        }

        public async Task<HttpResult<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/financial-service/api/categories/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<CategoryDto>>();
            return result ?? new HttpResult<CategoryDto> { Success = false };
        }

        public async Task<HttpResult<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/financial-service/api/categories", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<CategoryDto>>();
            return result ?? new HttpResult<CategoryDto> { Success = false };
        }

        public async Task<HttpResult> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/financial-service/api/categories/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/financial-service/api/categories/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { Success = false };
        }
    }
}
