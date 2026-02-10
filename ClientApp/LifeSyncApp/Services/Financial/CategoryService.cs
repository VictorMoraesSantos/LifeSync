using System.Net.Http.Json;
using System.Text.Json;
using LifeSyncApp.DTOs.Financial;

namespace LifeSyncApp.Services.Financial
{
    public class CategoryService
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
                var response = await client.GetAsync($"{BaseUrl}/user/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting categories. Status: {response.StatusCode}");
                    return new List<CategoryDTO>();
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<CategoryDTO>>>(_jsonOptions, cancellationToken);
                return result?.Data ?? new List<CategoryDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting categories: {ex.Message}");
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

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDTO>>(_jsonOptions, cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting category: {ex.Message}");
                return null;
            }
        }

        public async Task<int?> CreateCategoryAsync(CreateCategoryDTO dto, CancellationToken cancellationToken = default)
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
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>(_jsonOptions, cancellationToken);
                System.Diagnostics.Debug.WriteLine($"Category created with ID: {result?.Data}");
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating category: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                System.Diagnostics.Debug.WriteLine($"Deleting category {id}");

                var response = await client.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deleting category. Status: {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting category: {ex.Message}");
                return false;
            }
        }

        private class ApiResponse<T>
        {
            public T? Data { get; set; }
            public bool IsSuccess { get; set; }
            public string? Message { get; set; }
        }
    }
}
