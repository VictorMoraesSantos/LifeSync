using LifeSyncApp.DTOs.Common;
using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.DTOs.Nutrition.Food;
using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.LiquidType;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.DTOs.Nutrition.MealFood;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Services.Nutrition
{
    public class NutritionService
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

        public NutritionService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = jsonOptions;
        }

        private static async Task LogErrorAsync(HttpResponseMessage response, string context)
        {
            var body = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[NutritionService] {context} failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        // ── Diaries ──────────────────────────────────────────────────────────

        public async Task<List<DiaryDTO>> GetDiariesByUserIdAsync(int userId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{DiariesBase}/user/{userId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDiaries"); return []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<DiaryDTO>>>(_jsonOptions, ct);
                return result?.Data ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDiaries exception: {ex.Message}");
                return [];
            }
        }

        public async Task<DiaryDTO?> GetDiaryByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{DiariesBase}/{id}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDiaryById"); return null; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<DiaryDTO>>(_jsonOptions, ct);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDiaryById exception: {ex.Message}");
                return null;
            }
        }

        public async Task<int?> CreateDiaryAsync(CreateDiaryDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(DiariesBase, dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateDiary"); return null; }
                var json = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions, ct);
                if (json.TryGetProperty("data", out var data))
                {
                    if (data.ValueKind == JsonValueKind.Number)
                        return data.GetInt32();
                    if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty("id", out var idProp))
                        return idProp.GetInt32();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateDiary exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateDiaryAsync(int id, UpdateDiaryDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{DiariesBase}/{id}", dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "UpdateDiary"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateDiary exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteDiaryAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{DiariesBase}/{id}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "DeleteDiary"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] DeleteDiary exception: {ex.Message}");
                return false;
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
                if (!string.IsNullOrWhiteSpace(nameContains)) query += $"nameContains={Uri.EscapeDataString(nameContains)}&";
                if (page.HasValue) query += $"page={page.Value}&";
                if (pageSize.HasValue) query += $"pageSize={pageSize.Value}&";
                query = query.TrimEnd('&', '?');

                var response = await client.GetAsync($"{MealFoodsBase}/search{query}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "SearchFoods"); return []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<MealFoodDTO>>>(_jsonOptions, ct);
                var mealFoods = result?.Data ?? [];

                return mealFoods
                    .GroupBy(mf => mf.FoodId)
                    .Select(g => g.First())
                    .Select(mf => new FoodDTO(mf.FoodId, mf.Name, mf.Calories, mf.Protein, mf.Lipids, mf.Carbohydrates, mf.Calcium, mf.Magnesium, mf.Iron, mf.Sodium, mf.Potassium))
                    .ToList();
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
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{MealsBase}/diary/{diaryId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetMeals"); return []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<MealDTO>>>(_jsonOptions, ct);
                return result?.Data ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetMeals exception: {ex.Message}");
                return [];
            }
        }

        public async Task<MealDTO?> GetMealByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{MealsBase}/{id}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetMealById"); return null; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<MealDTO>>(_jsonOptions, ct);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetMealById exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateMealAsync(CreateMealDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(MealsBase, dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateMeal exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMealAsync(int mealId, UpdateMealDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{MealsBase}/{mealId}", dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "UpdateMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateMeal exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteMealAsync(int mealId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{MealsBase}/{mealId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "DeleteMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] DeleteMeal exception: {ex.Message}");
                return false;
            }
        }

        // ── Meal Foods ────────────────────────────────────────────────────────

        public async Task<bool> AddFoodToMealAsync(int mealId, CreateMealFoodDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync($"{MealsBase}/{mealId}/foods", dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "AddFoodToMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] AddFoodToMeal exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFoodFromMealAsync(int mealId, int foodId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{MealsBase}/{mealId}/foods/{foodId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "RemoveFoodFromMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] RemoveFoodFromMeal exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMealFoodAsync(int id, UpdateMealFoodDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{MealFoodsBase}/{id}", dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "UpdateMealFood"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateMealFood exception: {ex.Message}");
                return false;
            }
        }

        // ── Liquid Types ────────────────────────────────────────────────────

        public async Task<List<LiquidTypeDTO>> GetLiquidTypesAsync(CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync(LiquidTypesBase, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetLiquidTypes"); return []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<LiquidTypeDTO>>>(_jsonOptions, ct);
                return result?.Data ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetLiquidTypes exception: {ex.Message}");
                return [];
            }
        }

        // ── Liquids ───────────────────────────────────────────────────────────

        public async Task<List<LiquidDTO>> GetLiquidsByDiaryIdAsync(int diaryId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{LiquidsBase}/diary/{diaryId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetLiquids"); return []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<LiquidDTO>>>(_jsonOptions, ct);
                return result?.Data ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetLiquids exception: {ex.Message}");
                return [];
            }
        }

        public async Task<bool> CreateLiquidAsync(CreateLiquidDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(LiquidsBase, dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateLiquid"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateLiquid exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateLiquidAsync(int liquidId, UpdateLiquidDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{LiquidsBase}/{liquidId}", dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "UpdateLiquid"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateLiquid exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLiquidAsync(int liquidId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{LiquidsBase}/{liquidId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "DeleteLiquid"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] DeleteLiquid exception: {ex.Message}");
                return false;
            }
        }

        // ── Daily Progress / Goals ────────────────────────────────────────────

        public async Task<List<DailyProgressDTO>> GetDailyProgressByUserIdAsync(int userId, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{ProgressBase}/user/{userId}", ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDailyProgress"); return []; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<DailyProgressDTO>>>(_jsonOptions, ct);
                return result?.Data ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDailyProgress exception: {ex.Message}");
                return [];
            }
        }

        public async Task<int?> CreateDailyProgressAsync(CreateDailyProgressDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(ProgressBase, dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateDailyProgress"); return null; }
                var json = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions, ct);
                if (json.TryGetProperty("data", out var data))
                {
                    if (data.ValueKind == JsonValueKind.Number)
                        return data.GetInt32();
                    if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty("id", out var idProp))
                        return idProp.GetInt32();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateDailyProgress exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SetGoalAsync(int dailyProgressId, SetGoalDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync($"{ProgressBase}/{dailyProgressId}/set-goal", dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "SetGoal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] SetGoal exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDailyProgressAsync(int id, UpdateDailyProgressDTO dto, CancellationToken ct = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{ProgressBase}/{id}", dto, _jsonOptions, ct);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "UpdateDailyProgress"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateDailyProgress exception: {ex.Message}");
                return false;
            }
        }
    }
}
