using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.Nutrition;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Client.Services
{
    public class NutritionService : INutritionService
    {
        private readonly HttpClient _httpClient;

        public NutritionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResult<List<DiaryDto>>> GetDiariesAsync(int? userId = null)
        {
            var url = userId.HasValue
                ? $"/nutrition-service/api/diaries/user/{userId.Value}"
                : "/nutrition-service/api/diaries";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<DiaryDto>>>();
            return result ?? new HttpResult<List<DiaryDto>> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<DiaryDto>> GetDiaryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/nutrition-service/api/diaries/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<DiaryDto>>();
            return result ?? new HttpResult<DiaryDto> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<int>> CreateDiaryAsync(CreateDiaryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/nutrition-service/api/diaries", request);

            // Try expected shape: HttpResult<int>
            try
            {
                var resultInt = await response.Content.ReadFromJsonAsync<HttpResult<int>>();
                if (resultInt != null) return resultInt;
            }
            catch { /* ignore and try other shapes */ }

            // Try alternative shape: HttpResult<DiaryDto> (map Id)
            try
            {
                var resultDto = await response.Content.ReadFromJsonAsync<HttpResult<DiaryDto>>();
                if (resultDto != null)
                {
                    return new HttpResult<int>
                    {
                        StatusCode = resultDto.StatusCode,
                        Errors = resultDto.Errors ?? Array.Empty<string>(),
                        Data = resultDto.Data?.Id ?? 0
                    };
                }
            }
            catch { /* ignore and try raw json */ }

            // Fallback: read raw JSON and extract data/id
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                    return new HttpResult<int> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response body" } };

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                int id = 0;
                if (root.TryGetProperty("data", out var dataElem))
                {
                    if (dataElem.ValueKind == JsonValueKind.Number && dataElem.TryGetInt32(out var num))
                        id = num;
                    else if (dataElem.ValueKind == JsonValueKind.Object &&
                             dataElem.TryGetProperty("id", out var idProp) &&
                             idProp.TryGetInt32(out var objId))
                        id = objId;
                }

                string[] errors = Array.Empty<string>();
                if (root.TryGetProperty("errors", out var errorsElem) && errorsElem.ValueKind == JsonValueKind.Array)
                {
                    errors = errorsElem
                        .EnumerateArray()
                        .Where(e => e.ValueKind == JsonValueKind.String)
                        .Select(e => e.GetString()!)
                        .ToArray();
                }

                return new HttpResult<int>
                {
                    StatusCode = (int)response.StatusCode,
                    Errors = errors,
                    Data = id
                };
            }
            catch
            {
                return new HttpResult<int>
                {
                    StatusCode = (int)response.StatusCode,
                    Errors = new[] { "Failed to parse server response." }
                };
            }
        }

        public async Task<HttpResult> UpdateDiaryAsync(int id, DateOnly date)
        {
            var request = new { Date = date };
            var response = await _httpClient.PutAsJsonAsync($"/nutrition-service/api/diaries/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> DeleteDiaryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/nutrition-service/api/diaries/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<List<MealDto>>> GetMealsAsync(int? diaryId = null)
        {
            var url = diaryId.HasValue
                ? $"/nutrition-service/api/meals/diary/{diaryId.Value}"
                : "/nutrition-service/api/meals";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<MealDto>>>();
            return result ?? new HttpResult<List<MealDto>> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<MealDto>> GetMealByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/nutrition-service/api/meals/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<MealDto>>();
            return result ?? new HttpResult<MealDto> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<MealDto>> CreateMealAsync(CreateMealRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/nutrition-service/api/meals", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<MealDto>>();
            return result ?? new HttpResult<MealDto> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> UpdateMealAsync(int id, UpdateMealRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/nutrition-service/api/meals/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> DeleteMealAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/nutrition-service/api/meals/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<MealDto>> AddMealFoodAsync(int mealId, CreateMealFoodRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"/nutrition-service/api/meals/{mealId}/foods", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<MealDto>>();
            return result ?? new HttpResult<MealDto> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> RemoveMealFoodAsync(int mealId, int foodId)
        {
            var response = await _httpClient.DeleteAsync($"/nutrition-service/api/meals/{mealId}/foods/{foodId}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<List<LiquidDto>>> GetLiquidsAsync(int? diaryId = null)
        {
            var url = diaryId.HasValue
                ? $"/nutrition-service/api/liquids/diary/{diaryId.Value}"
                : "/nutrition-service/api/liquids";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<LiquidDto>>>();
            return result ?? new HttpResult<List<LiquidDto>> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<LiquidDto>> GetLiquidByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/nutrition-service/api/liquids/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<LiquidDto>>();
            return result ?? new HttpResult<LiquidDto> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<LiquidDto>> CreateLiquidAsync(CreateLiquidRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/nutrition-service/api/liquids", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<LiquidDto>>();
            return result ?? new HttpResult<LiquidDto> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> UpdateLiquidAsync(int id, UpdateLiquidRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/nutrition-service/api/liquids/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> DeleteLiquidAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/nutrition-service/api/liquids/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<List<DailyProgressDto>>> GetDailyProgressesAsync(int? userId = null)
        {
            var url = userId.HasValue
                ? $"/nutrition-service/api/daily-progresses/user/{userId.Value}"
                : "/nutrition-service/api/daily-progresses";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<List<DailyProgressDto>>>();
            return result ?? new HttpResult<List<DailyProgressDto>> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<DailyProgressDto>> GetDailyProgressByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/nutrition-service/api/daily-progresses/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult<DailyProgressDto>>();
            return result ?? new HttpResult<DailyProgressDto> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult<DailyProgressDto>> CreateDailyProgressAsync(CreateDailyProgressRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/nutrition-service/api/daily-progresses", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult<DailyProgressDto>>();
            return result ?? new HttpResult<DailyProgressDto> { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> UpdateDailyProgressAsync(int id, UpdateDailyProgressRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/nutrition-service/api/daily-progresses/{id}", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> DeleteDailyProgressAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/nutrition-service/api/daily-progresses/{id}");
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }

        public async Task<HttpResult> SetDailyProgressGoalAsync(int id, DailyGoalDto goal)
        {
            var request = new { Goal = goal };
            var response = await _httpClient.PostAsJsonAsync($"/nutrition-service/api/daily-progresses/{id}/set-goal", request);
            var result = await response.Content.ReadFromJsonAsync<HttpResult>();
            return result ?? new HttpResult { StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
        }
    }
}
