using LifeSyncApp.DTOs.Financial.Transaction;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Services.Financial
{
    public class TransactionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "/financial-service/api/transactions";

        public TransactionService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = jsonOptions;
        }

        public async Task<List<TransactionDTO>> GetTransactionsByUserIdAsync(int userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{BaseUrl}/user/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return new List<TransactionDTO>();

                var result =
                    await response.Content.ReadFromJsonAsync<ApiResponse<List<TransactionDTO>>>(_jsonOptions,
                        cancellationToken);
                return result?.Data ?? new List<TransactionDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting transactions: {ex.Message}");
                return new List<TransactionDTO>();
            }
        }

        public async Task<TransactionDTO?> GetTransactionByIdAsync(int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{BaseUrl}/{id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return null;

                var result =
                    await response.Content.ReadFromJsonAsync<ApiResponse<TransactionDTO>>(_jsonOptions,
                        cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting transaction: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TransactionDTO>> SearchTransactionsAsync(TransactionFilterDTO filter,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var queryString = BuildQueryString(filter);
                var response = await client.GetAsync($"{BaseUrl}/search?{queryString}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return new List<TransactionDTO>();

                var result =
                    await response.Content.ReadFromJsonAsync<ApiResponse<List<TransactionDTO>>>(_jsonOptions,
                        cancellationToken);
                return result?.Data ?? new List<TransactionDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching transactions: {ex.Message}");
                return new List<TransactionDTO>();
            }
        }

        public async Task<int?> CreateTransactionAsync(CreateTransactionDTO dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(BaseUrl, dto, _jsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine(
                        $"Error creating transaction. Status: {response.StatusCode}, Content: {errorContent}");
                    return null;
                }

                var result =
                    await response.Content.ReadFromJsonAsync<ApiResponse<int>>(_jsonOptions, cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating transaction: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{BaseUrl}/{id}", dto, _jsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine(
                        $"Error updating transaction. Status: {response.StatusCode}, Content: {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating transaction: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTransactionAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deleting transaction. Status: {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting transaction: {ex.Message}");
                return false;
            }
        }

        private string BuildQueryString(TransactionFilterDTO filter)
        {
            var parameters = new List<string>();

            if (filter.UserId.HasValue)
                parameters.Add($"UserId={filter.UserId}");
            if (filter.CategoryId.HasValue)
                parameters.Add($"CategoryId={filter.CategoryId}");
            if (filter.PaymentMethod.HasValue)
                parameters.Add($"PaymentMethod={filter.PaymentMethod}");
            if (filter.TransactionType.HasValue)
                parameters.Add($"TransactionType={filter.TransactionType}");
            if (filter.TransactionDateFrom.HasValue)
                parameters.Add($"TransactionDateFrom={filter.TransactionDateFrom:yyyy-MM-dd}");
            if (filter.TransactionDateTo.HasValue)
                parameters.Add($"TransactionDateTo={filter.TransactionDateTo:yyyy-MM-dd}");
            if (filter.Page.HasValue)
                parameters.Add($"Page={filter.Page}");
            if (filter.PageSize.HasValue)
                parameters.Add($"PageSize={filter.PageSize}");

            return string.Join("&", parameters);
        }

        private class ApiResponse<T>
        {
            public T? Data { get; set; }
            public bool IsSuccess { get; set; }
            public string? Message { get; set; }
        }
    }
}