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

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GET {Endpoint} retornou {StatusCode}: {Body}", endpoint, response.StatusCode, errorBody);

                    var friendlyMessage = TryExtractValidationErrors(errorBody)
                        ?? $"Erro {(int)response.StatusCode}: {response.ReasonPhrase}";
                    throw new HttpRequestException(friendlyMessage);
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiSingleResponse<T>>(_jsonOptions);
                var resultData = apiResponse.Data;
                return resultData;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
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

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("SEARCH {Endpoint} retornou {StatusCode}: {Body}", endpoint, response.StatusCode, errorBody);

                    var friendlyMessage = TryExtractValidationErrors(errorBody)
                        ?? $"Erro {(int)response.StatusCode}: {response.ReasonPhrase}";
                    throw new HttpRequestException(friendlyMessage);
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(_jsonOptions);
                var resultData = apiResponse?.Data ?? new List<T>();
                return resultData;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
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

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("POST {Endpoint} retornou {StatusCode}: {Body}", endpoint, response.StatusCode, errorBody);

                    var friendlyMessage = TryExtractValidationErrors(errorBody)
                        ?? $"Erro {(int)response.StatusCode}: {response.ReasonPhrase}";
                    throw new HttpRequestException(friendlyMessage);
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiSingleResponse<TResult>>(_jsonOptions);
                var resultData = apiResponse.Data;
                return resultData;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
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

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("PUT {Endpoint} retornou {StatusCode}: {Body}", endpoint, response.StatusCode, errorBody);

                    var friendlyMessage = TryExtractValidationErrors(errorBody)
                        ?? $"Erro {(int)response.StatusCode}: {response.ReasonPhrase}";
                    throw new HttpRequestException(friendlyMessage);
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer PUT em {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DELETE {Endpoint} retornou {StatusCode}: {Body}", endpoint, response.StatusCode, errorBody);

                    var friendlyMessage = TryExtractValidationErrors(errorBody)
                        ?? $"Erro {(int)response.StatusCode}: {response.ReasonPhrase}";
                    throw new HttpRequestException(friendlyMessage);
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer DELETE em {Endpoint}", endpoint);
                throw;
            }
        }

        private static string? TryExtractValidationErrors(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errors))
                {
                    var messages = new List<string>();

                    if (errors.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in errors.EnumerateArray())
                            if (item.GetString() is string msg)
                                messages.Add(msg);
                    }
                    else if (errors.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var prop in errors.EnumerateObject())
                            foreach (var msg in prop.Value.EnumerateArray())
                                messages.Add(msg.GetString() ?? prop.Name);
                    }

                    if (messages.Count > 0)
                        return string.Join("\n", messages);
                }

                if (root.TryGetProperty("description", out var description))
                    return description.GetString();

                if (root.TryGetProperty("title", out var title))
                    return title.GetString();
            }
            catch
            {
                // Not a JSON response or unexpected format
            }

            return null;
        }
    }
}
