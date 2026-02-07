using LifeSyncApp.DTOs.Common;
using LifeSyncApp.Services.ApiService.Interface;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Services.ApiService.Implementation
{
    public class ApiService<T> : IApiService<T>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService<T>> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(IHttpClientFactory httpClient, ILogger<ApiService<T>> logger, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClient.CreateClient("LifeSyncApi");
            _logger = logger;
            _jsonOptions = jsonOptions;
        }

        public async Task<T> GetAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiSingleResponse<T>>(_jsonOptions);
                var resultData = apiResponse.Data;
                return resultData;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao fazer GET em {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<IEnumerable<T>> SearchAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(_jsonOptions);
                var resultData = apiResponse?.Data ?? new List<T>();
                return resultData;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao fazer SEARCH em {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<TResult> PostAsync<TResult>(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
                response.EnsureSuccessStatusCode();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiSingleResponse<TResult>>(_jsonOptions);
                var resultData = apiResponse.Data;
                return resultData;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao fazer POST em {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task PutAsync(string endpoint, object data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions);
                if (response.IsSuccessStatusCode)
                    return;

                return;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao fazer PUT em {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task DeleteAsync(string endpoint)
        {
            try
            {
                await _httpClient.DeleteAsync(endpoint);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao fazer DELETE em {Endpoint}", endpoint);
                throw;
            }
        }
    }
}
