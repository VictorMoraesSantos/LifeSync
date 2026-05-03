using LifeSyncApp.DTOs.Common;
using LifeSyncApp.DTOs.Financial.Category;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Services.Financial
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "/financial-service/api/categories";

        public CategoryService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = jsonOptions;
        }

        public async Task<List<CategoryDTO>> GetCategoriesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var url = $"{BaseUrl}/user/{userId}";
                System.Diagnostics.Debug.WriteLine($"[CategoryService] GET {client.BaseAddress}{url}");
                var response = await client.GetAsync(url, cancellationToken);

                System.Diagnostics.Debug.WriteLine($"[CategoryService] Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"[CategoryService] Error body: {errorBody}");
                    return new List<CategoryDTO>();
                }

                var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);
                System.Diagnostics.Debug.WriteLine($"[CategoryService] Response JSON: {rawJson[..Math.Min(500, rawJson.Length)]}");

                var result = JsonSerializer.Deserialize<ApiSingleResponse<List<CategoryDTO>>>(rawJson, _jsonOptions);
                System.Diagnostics.Debug.WriteLine($"[CategoryService] Deserialized {result?.Data?.Count ?? 0} categories");
                return result?.Data ?? new List<CategoryDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategoryService] Error getting categories: {ex.Message}\n{ex.StackTrace}");
                return new List<CategoryDTO>();
            }
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{BaseUrl}/{id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting category. Status: {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<CategoryDTO>>(_jsonOptions, cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting category: {ex.Message}");
                return null;
            }
        }

        public async Task<(int? Id, string? Error)> CreateCategoryAsync(CreateCategoryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                System.Diagnostics.Debug.WriteLine($"Creating category: {dto.Name}");

                var response = await client.PostAsJsonAsync(BaseUrl, dto, _jsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"Error creating category. Status: {response.StatusCode}, Content: {errorContent}");
                    return (null, ExtractErrorMessage(errorContent));
                }

                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<int>>(_jsonOptions, cancellationToken);
                System.Diagnostics.Debug.WriteLine($"Category created with ID: {result?.Data}");
                return (result?.Data, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating category: {ex.Message}\n{ex.StackTrace}");
                return (null, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> UpdateCategoryAsync(int id, UpdateCategoryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                System.Diagnostics.Debug.WriteLine($"Updating category {id}: {dto.Name}");

                var response = await client.PutAsJsonAsync($"{BaseUrl}/{id}", dto, _jsonOptions, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"Error updating category. Status: {response.StatusCode}, Content: {errorContent}");
                    return (false, ExtractErrorMessage(errorContent));
                }

                System.Diagnostics.Debug.WriteLine($"Category {id} updated successfully");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating category: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                System.Diagnostics.Debug.WriteLine($"Deleting category {id}");

                var response = await client.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"Error deleting category. Status: {response.StatusCode}, Content: {errorContent}");
                    return (false, ExtractErrorMessage(errorContent));
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting category: {ex.Message}");
                return (false, ex.Message);
            }
        }

        private static string? ExtractErrorMessage(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errors))
                {
                    if (errors.ValueKind == JsonValueKind.Array)
                    {
                        var messages = new List<string>();
                        foreach (var item in errors.EnumerateArray())
                            if (item.GetString() is string msg)
                                messages.Add(msg);
                        if (messages.Count > 0)
                            return string.Join("\n", messages);
                    }
                    else if (errors.ValueKind == JsonValueKind.Object)
                    {
                        var messages = new List<string>();
                        foreach (var prop in errors.EnumerateObject())
                            foreach (var msg in prop.Value.EnumerateArray())
                                messages.Add(msg.GetString() ?? prop.Name);
                        if (messages.Count > 0)
                            return string.Join("\n", messages);
                    }
                }
                if (root.TryGetProperty("description", out var desc) && desc.GetString() is string d)
                    return d;
                if (root.TryGetProperty("title", out var title) && title.GetString() is string t)
                    return t;
            }
            catch { }
            return null;
        }
    }
}
