using LifeSyncApp.DTOs.Common;
using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.DTOs.Nutrition.Food;
using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.LiquidType;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.DTOs.Nutrition.MealFood;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Services.Nutrition
{
    public class NutritionService : INutritionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string DiariesBase = "/nutrition-service/api/diaries";
        private const string MealsBase = "/nutrition-service/api/meals";
        private const string MealFoodsBase = "/nutrition-service/api/meal-foods";
        private const string FoodsBase = "/nutrition-service/api/foods";
        private const string LiquidsBase = "/nutrition-service/api/liquids";
        private const string LiquidTypesBase = "/nutrition-service/api/liquid-types";
        private const string ProgressBase = "/nutrition-service/api/daily-progresses";

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

        public NutritionService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = jsonOptions;
        }

        // ── Cache ───────────────────────────────────────────────────────────────

        private sealed record CacheEntry(object Data, DateTime CachedAt);

        private T? GetFromCache<T>(string key)
        {
            if (_cache.TryGetValue(key, out var entry) && entry.Data is T data
                && (DateTime.Now - entry.CachedAt) < CacheDuration)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] Cache hit: {key}");
                return data;
            }
            return default;
        }

        private T? GetStaleFromCache<T>(string key)
        {
            if (_cache.TryGetValue(key, out var entry) && entry.Data is T data)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] Using stale cache (fallback): {key}");
                return data;
            }
            return default;
        }

        private void SetCache<T>(string key, T data) where T : notnull
        {
            _cache[key] = new CacheEntry(data, DateTime.Now);
        }

        private void InvalidateCache(string prefix)
        {
            var keysToRemove = _cache.Keys.Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in keysToRemove)
                _cache.TryRemove(key, out _);
        }

        public void InvalidateAllCache()
        {
            _cache.Clear();
            System.Diagnostics.Debug.WriteLine("[NutritionService] All cache invalidated");
        }

        private static async Task LogErrorAsync(HttpResponseMessage response, string context)
        {
            var body = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[NutritionService] {context} failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        /// <summary>
        /// Checks if the response indicates success by reading the body's success flag.
        /// Returns (true, null) on success or (false, errorMessage) on failure.
        /// </summary>
        private async Task<(bool Success, string? Error)> CheckResponseAsync(HttpResponseMessage response, string context, CancellationToken ct = default)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<ApiSingleResponse<object>>(body, _jsonOptions);
            if (result?.Success == true) return (true, null);

            System.Diagnostics.Debug.WriteLine($"[NutritionService] {context} failed. Body: {body}");
            var error = result?.Errors?.FirstOrDefault()
                ?? ExtractErrorMessage(body);
            return (false, error);
        }

        // ── Diaries ──────────────────────────────────────────────────────────

        public async Task<List<DiaryDTO>> GetDiariesByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var cacheKey = $"diaries_user_{userId}";
            var cached = GetFromCache<List<DiaryDTO>>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{DiariesBase}/user/{userId}", ct);
                if (!response.IsSuccessStatusCode) { return GetStaleFromCache<List<DiaryDTO>>(cacheKey) ?? []; }
                var rawBody = await response.Content.ReadAsStringAsync(ct);
                var result = JsonSerializer.Deserialize<ApiSingleResponse<List<DiaryDTO>>>(rawBody, _jsonOptions);
                var data = result?.Data ?? [];
                SetCache(cacheKey, data);
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDiaries exception: {ex.Message}");
                return GetStaleFromCache<List<DiaryDTO>>(cacheKey) ?? [];
            }
        }

        public async Task<DiaryDTO?> GetDiaryByIdAsync(int id, CancellationToken ct = default)
        {
            var cacheKey = $"diary_{id}";
            var cached = GetFromCache<DiaryDTO>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{DiariesBase}/{id}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDiaryById"); return GetStaleFromCache<DiaryDTO>(cacheKey); }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<DiaryDTO>>(_jsonOptions, ct);
                if (result?.Data != null) SetCache(cacheKey, result.Data);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDiaryById exception: {ex.Message}");
                return GetStaleFromCache<DiaryDTO>(cacheKey);
            }
        }

        public async Task<DiaryDTO?> GetDiaryByDateAsync(int userId, DateOnly date, CancellationToken ct = default)
        {
            var cacheKey = $"diary_user_{userId}_date_{date:yyyy-MM-dd}";
            var cached = GetFromCache<DiaryDTO>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{DiariesBase}/user/{userId}/date/{date:yyyy-MM-dd}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDiaryByDate"); return GetStaleFromCache<DiaryDTO>(cacheKey); }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<DiaryDTO>>(_jsonOptions, ct);
                if (result?.Data != null) SetCache(cacheKey, result.Data);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDiaryByDate exception: {ex.Message}");
                return GetStaleFromCache<DiaryDTO>(cacheKey);
            }
        }

        public async Task<(int? Id, string? Error)> CreateDiaryAsync(CreateDiaryDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(DiariesBase, dto, _jsonOptions, ct);
                var body = await response.Content.ReadAsStringAsync(ct);
                var result = JsonSerializer.Deserialize<ApiSingleResponse<JsonElement?>>(body, _jsonOptions);

                // Check both HTTP status and body success flag
                if (!response.IsSuccessStatusCode || result?.Success == false)
                {
                    var errorMsg = result?.Errors?.FirstOrDefault()
                        ?? ExtractErrorMessage(body)
                        ?? $"Erro do servidor ({(int)response.StatusCode})";
                    return (null, errorMsg);
                }

                if (result?.Data != null)
                {
                    var data = result.Data.Value;
                    InvalidateCache("diaries_");
                    if (data.ValueKind == JsonValueKind.Number)
                        return (data.GetInt32(), null);
                    if (data.ValueKind == JsonValueKind.Object &&
                        (data.TryGetProperty("id", out var idProp) || data.TryGetProperty("Id", out idProp)))
                        return (idProp.GetInt32(), null);
                }

                InvalidateCache("diaries_");
                return (null, "Resposta inesperada do servidor.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateDiary exception: {ex.Message}");
                return (null, ex.Message);
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

        public async Task<(bool Success, string? Error)> UpdateDiaryAsync(int id, UpdateDiaryDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{DiariesBase}/{id}", dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(ct);
                    System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateDiary failed. Status: {(int)response.StatusCode}. Body: {body}");
                    return (false, ExtractErrorMessage(body));
                }
                InvalidateCache("diaries_");
                InvalidateCache($"diary_{id}");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateDiary exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteDiaryAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{DiariesBase}/{id}", ct);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(ct);
                    System.Diagnostics.Debug.WriteLine($"[NutritionService] DeleteDiary failed. Status: {(int)response.StatusCode}. Body: {body}");
                    return (false, ExtractErrorMessage(body));
                }
                InvalidateCache("diaries_");
                InvalidateCache($"diary_{id}");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] DeleteDiary exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<List<DiaryDTO>> SearchDiariesAsync(int userId, DateOnly? dateFrom = null, DateOnly? dateTo = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var query = $"?userId={userId}";
                if (dateFrom.HasValue) query += $"&dateFrom={dateFrom.Value:yyyy-MM-dd}";
                if (dateTo.HasValue) query += $"&dateTo={dateTo.Value:yyyy-MM-dd}";
                if (page.HasValue) query += $"&page={page.Value}";
                if (pageSize.HasValue) query += $"&pageSize={pageSize.Value}";
                query += "&sortBy=date&sortDesc=true";

                var response = await client.GetAsync($"{DiariesBase}/search{query}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "SearchDiaries"); return []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<DiaryDTO>>>(_jsonOptions, ct);
                return result?.Data ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] SearchDiaries exception: {ex.Message}");
                return [];
            }
        }

        // ── Foods (Catalog) ─────────────────────────────────────────────────

        public async Task<List<FoodDTO>> SearchFoodsAsync(string? nameContains = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var query = "?";
                if (!string.IsNullOrWhiteSpace(nameContains)) query += $"name={Uri.EscapeDataString(nameContains)}&";
                if (page.HasValue) query += $"page={page.Value}&";
                if (pageSize.HasValue) query += $"pageSize={pageSize.Value}&";
                query = query.TrimEnd('&', '?');

                var response = await client.GetAsync($"{FoodsBase}/search{query}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "SearchFoods"); return []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<FoodDTO>>>(_jsonOptions, ct);
                return result?.Data ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] SearchFoods exception: {ex.Message}");
                return [];
            }
        }

        // ── Meals ─────────────────────────────────────────────────────────────

        public async Task<List<MealDTO>> GetMealsByDiaryIdAsync(int diaryId, CancellationToken ct = default)
        {
            var cacheKey = $"meals_diary_{diaryId}";
            var cached = GetFromCache<List<MealDTO>>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{MealsBase}/diary/{diaryId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetMeals"); return GetStaleFromCache<List<MealDTO>>(cacheKey) ?? []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<MealDTO>>>(_jsonOptions, ct);
                var data = result?.Data ?? [];
                SetCache(cacheKey, data);
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetMeals exception: {ex.Message}");
                return GetStaleFromCache<List<MealDTO>>(cacheKey) ?? [];
            }
        }

        public async Task<MealDTO?> GetMealByIdAsync(int id, CancellationToken ct = default)
        {
            var cacheKey = $"meal_{id}";
            var cached = GetFromCache<MealDTO>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{MealsBase}/{id}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetMealById"); return GetStaleFromCache<MealDTO>(cacheKey); }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<MealDTO>>(_jsonOptions, ct);
                if (result?.Data != null) SetCache(cacheKey, result.Data);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetMealById exception: {ex.Message}");
                return GetStaleFromCache<MealDTO>(cacheKey);
            }
        }

        public async Task<(bool Success, string? Error)> CreateMealAsync(CreateMealDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(MealsBase, dto, _jsonOptions, ct);
                var (success, error) = await CheckResponseAsync(response, "CreateMeal", ct);
                if (!success) return (false, error);
                InvalidateCache("meals_diary_");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateMeal exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> UpdateMealAsync(int mealId, UpdateMealDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{MealsBase}/{mealId}", dto, _jsonOptions, ct);
                var (success, error) = await CheckResponseAsync(response, "UpdateMeal", ct);
                if (!success) return (false, error);
                InvalidateCache("meals_diary_");
                InvalidateCache($"meal_{mealId}");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateMeal exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteMealAsync(int mealId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{MealsBase}/{mealId}", ct);
                var (success, error) = await CheckResponseAsync(response, "DeleteMeal", ct);
                if (!success) return (false, error);
                InvalidateCache("meals_diary_");
                InvalidateCache($"meal_{mealId}");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] DeleteMeal exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        // ── Meal Foods ────────────────────────────────────────────────────────

        public async Task<(bool Success, string? Error)> AddFoodToMealAsync(int mealId, CreateMealFoodDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var url = $"{MealsBase}/{mealId}/foods";
                var response = await client.PostAsJsonAsync(url, dto, _jsonOptions, ct);
                var (success, error) = await CheckResponseAsync(response, "AddFoodToMeal", ct);
                if (!success) return (false, error);

                InvalidateCache("meals_diary_");
                InvalidateCache($"meal_{mealId}");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] AddFoodToMeal exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> RemoveFoodFromMealAsync(int mealId, int foodId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{MealsBase}/{mealId}/foods/{foodId}", ct);
                var (success, error) = await CheckResponseAsync(response, "RemoveFoodFromMeal", ct);
                if (!success) return (false, error);
                InvalidateCache("meals_diary_");
                InvalidateCache($"meal_{mealId}");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] RemoveFoodFromMeal exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> UpdateMealFoodAsync(int id, UpdateMealFoodDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{MealFoodsBase}/{id}", dto, _jsonOptions, ct);
                var (success, error) = await CheckResponseAsync(response, "UpdateMealFood", ct);
                if (!success) return (false, error);
                InvalidateCache("meals_diary_");
                InvalidateCache("meal_");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateMealFood exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        // ── Liquid Types ────────────────────────────────────────────────────

        public async Task<List<LiquidTypeDTO>> GetLiquidTypesAsync(CancellationToken ct = default)
        {
            var cacheKey = "liquid_types";
            var cached = GetFromCache<List<LiquidTypeDTO>>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync(LiquidTypesBase, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetLiquidTypes"); return GetStaleFromCache<List<LiquidTypeDTO>>(cacheKey) ?? []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<LiquidTypeDTO>>>(_jsonOptions, ct);
                var data = result?.Data ?? [];
                SetCache(cacheKey, data);
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetLiquidTypes exception: {ex.Message}");
                return GetStaleFromCache<List<LiquidTypeDTO>>(cacheKey) ?? [];
            }
        }

        // ── Liquids ───────────────────────────────────────────────────────────

        public async Task<List<LiquidDTO>> GetLiquidsByDiaryIdAsync(int diaryId, CancellationToken ct = default)
        {
            var cacheKey = $"liquids_diary_{diaryId}";
            var cached = GetFromCache<List<LiquidDTO>>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{LiquidsBase}/diary/{diaryId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetLiquids"); return GetStaleFromCache<List<LiquidDTO>>(cacheKey) ?? []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<LiquidDTO>>>(_jsonOptions, ct);
                var data = result?.Data ?? [];
                SetCache(cacheKey, data);
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetLiquids exception: {ex.Message}");
                return GetStaleFromCache<List<LiquidDTO>>(cacheKey) ?? [];
            }
        }

        public async Task<(bool Success, string? Error)> CreateLiquidAsync(CreateLiquidDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(LiquidsBase, dto, _jsonOptions, ct);
                var (success, error) = await CheckResponseAsync(response, "CreateLiquid", ct);
                if (!success) return (false, error);
                InvalidateCache("liquids_diary_");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateLiquid exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> UpdateLiquidAsync(int liquidId, UpdateLiquidDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{LiquidsBase}/{liquidId}", dto, _jsonOptions, ct);
                var (success, error) = await CheckResponseAsync(response, "UpdateLiquid", ct);
                if (!success) return (false, error);
                InvalidateCache("liquids_diary_");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateLiquid exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteLiquidAsync(int liquidId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{LiquidsBase}/{liquidId}", ct);
                var (success, error) = await CheckResponseAsync(response, "DeleteLiquid", ct);
                if (!success) return (false, error);
                InvalidateCache("liquids_diary_");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] DeleteLiquid exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        // ── Daily Progress / Goals ────────────────────────────────────────────

        public async Task<List<DailyProgressDTO>> GetDailyProgressByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var cacheKey = $"progress_user_{userId}";
            var cached = GetFromCache<List<DailyProgressDTO>>(cacheKey);
            if (cached != null) return cached;

            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{ProgressBase}/user/{userId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDailyProgress"); return GetStaleFromCache<List<DailyProgressDTO>>(cacheKey) ?? []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<DailyProgressDTO>>>(_jsonOptions, ct);
                var data = result?.Data ?? [];
                SetCache(cacheKey, data);
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDailyProgress exception: {ex.Message}");
                return GetStaleFromCache<List<DailyProgressDTO>>(cacheKey) ?? [];
            }
        }

        public async Task<int?> CreateDailyProgressAsync(CreateDailyProgressDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(ProgressBase, dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateDailyProgress"); return null; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<object>>(_jsonOptions, ct);
                if (result?.Data != null)
                {
                    InvalidateCache("progress_");
                    if (result.Data is JsonElement je)
                    {
                        if (je.ValueKind == JsonValueKind.Number)
                            return je.GetInt32();
                        if (je.ValueKind == JsonValueKind.Object && je.TryGetProperty("id", out var idProp))
                            return idProp.GetInt32();
                    }
                    if (int.TryParse(result.Data.ToString(), out var id))
                        return id;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateDailyProgress exception: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string? Error)> SetGoalAsync(int dailyProgressId, SetGoalDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync($"{ProgressBase}/{dailyProgressId}/set-goal", dto, _jsonOptions, ct);
                var (success, error) = await CheckResponseAsync(response, "SetGoal", ct);
                if (!success) return (false, error);
                InvalidateCache("progress_");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] SetGoal exception: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Error)> UpdateDailyProgressAsync(int id, UpdateDailyProgressDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{ProgressBase}/{id}", dto, _jsonOptions, ct);
                var (success, error) = await CheckResponseAsync(response, "UpdateDailyProgress", ct);
                if (!success) return (false, error);
                InvalidateCache("progress_");
                return (true, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateDailyProgress exception: {ex.Message}");
                return (false, ex.Message);
            }
        }
    }
}
