using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Services.Http;
using System.Net.Http.Json;
using LifeSyncApp.Client.Models.Financial;

namespace LifeSyncApp.Client.Services
{
    public interface IFinancialService
    {
        Task<ApiResponse<List<TransactionDTO>>> GetTransactionsAsync();
        Task<ApiResponse<int>> CreateTransactionAsync(CreateTransactionCommand command);
        Task<ApiResponse<bool>> UpdateTransactionAsync(UpdateTransactionCommand command);
        Task<ApiResponse<object>> DeleteTransactionAsync(int id);
        Task<ApiResponse<List<CategoryDTO>>> GetCategoriesAsync();
        Task<ApiResponse<int>> CreateCategoryAsync(CreateCategoryCommand command);
        Task<ApiResponse<bool>> UpdateCategoryAsync(UpdateCategoryCommand command);
        Task<ApiResponse<object>> DeleteCategoryAsync(int id);

        Task<ApiResponse<List<TransactionDTO>>> SearchTransactionsAsync(object filter);
        Task<ApiResponse<List<CategoryDTO>>> SearchCategoriesAsync(object filter);
        Task<ApiResponse<List<TransactionDTO>>> GetTransactionsByUserAsync(int userId);
        Task<ApiResponse<List<CategoryDTO>>> GetCategoriesByUserAsync(int userId);
    }

    public class FinancialService : IFinancialService
    {
        private readonly IApiClient _apiClient;

        public FinancialService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<TransactionDTO>>> GetTransactionsAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TransactionDTO>>>("financial-service/api/transactions");
                return res ?? new ApiResponse<List<TransactionDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TransactionDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateTransactionAsync(CreateTransactionCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateTransactionCommand, ApiResponse<int>>("financial-service/api/transactions", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateTransactionAsync(UpdateTransactionCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateTransactionCommand, ApiResponse<bool>>($"financial-service/api/transactions/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteTransactionAsync(int id)
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

        public async Task<ApiResponse<List<CategoryDTO>>> GetCategoriesAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<CategoryDTO>>>("financial-service/api/categories");
                return res ?? new ApiResponse<List<CategoryDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateCategoryAsync(CreateCategoryCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateCategoryCommand, ApiResponse<int>>("financial-service/api/categories", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateCategoryAsync(UpdateCategoryCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateCategoryCommand, ApiResponse<bool>>($"financial-service/api/categories/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteCategoryAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"financial-service/api/categories/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TransactionDTO>>> SearchTransactionsAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<TransactionDTO>>>($"financial-service/api/transactions/search{query}");
                return res ?? new ApiResponse<List<TransactionDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TransactionDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<CategoryDTO>>> SearchCategoriesAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<CategoryDTO>>>($"financial-service/api/categories/search{query}");
                return res ?? new ApiResponse<List<CategoryDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<TransactionDTO>>> GetTransactionsByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<TransactionDTO>>>($"financial-service/api/transactions/user/{userId}");
                return res ?? new ApiResponse<List<TransactionDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<TransactionDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<CategoryDTO>>> GetCategoriesByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<CategoryDTO>>>($"financial-service/api/categories/user/{userId}");
                return res ?? new ApiResponse<List<CategoryDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }
    }
}
