using LifeSyncApp.DTOs.Common;
using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.DTOs.Nutrition.Liquid;
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
        private const string LiquidsBase = "/nutrition-service/api/liquids";
        private const string ProgressBase = "/nutrition-service/api/daily-progresses";

        public NutritionService(IHttpClientFactory httpClientFactory, JsonSerializerOptions jsonOptions)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = jsonOptions;
        }

        // ── Diaries ──────────────────────────────────────────────────────────

        private static async Task LogErrorAsync(HttpResponseMessage response, string context)
        {
            var body = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[NutritionService] {context} failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
        }

        public async Task<List<DiaryDTO>> GetDiariesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{DiariesBase}/user/{userId}", cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDiaries"); return new List<DiaryDTO>(); }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<DiaryDTO>>>(_jsonOptions, cancellationToken);
                return result?.Data ?? new List<DiaryDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDiaries exception: {ex.Message}");
                return new List<DiaryDTO>();
            }
        }

        public async Task<DiaryDTO?> GetDiaryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{DiariesBase}/{id}", cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDiaryById"); return null; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<DiaryDTO>>(_jsonOptions, cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDiaryById exception: {ex.Message}");
                return null;
            }
        }

        public async Task<int?> CreateDiaryAsync(CreateDiaryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(DiariesBase, dto, _jsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateDiary"); return null; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<int>>(_jsonOptions, cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateDiary exception: {ex.Message}");
                return null;
            }
        }

        // ── Meals ─────────────────────────────────────────────────────────────

        public async Task<List<MealDTO>> GetMealsByDiaryIdAsync(int diaryId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{MealsBase}/diary/{diaryId}", cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetMeals"); return new List<MealDTO>(); }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<MealDTO>>>(_jsonOptions, cancellationToken);
                return result?.Data ?? new List<MealDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetMeals exception: {ex.Message}");
                return new List<MealDTO>();
            }
        }

        public async Task<MealDTO?> GetMealByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{MealsBase}/{id}", cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetMealById"); return null; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<MealDTO>>(_jsonOptions, cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetMealById exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateMealAsync(CreateMealDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(MealsBase, dto, _jsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateMeal exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMealAsync(int mealId, UpdateMealDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{MealsBase}/{mealId}", dto, _jsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "UpdateMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateMeal exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteMealAsync(int mealId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{MealsBase}/{mealId}", cancellationToken);
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

        public async Task<bool> AddFoodToMealAsync(int mealId, CreateMealFoodDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync($"{MealsBase}/{mealId}/foods", dto, _jsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "AddFoodToMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] AddFoodToMeal exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFoodFromMealAsync(int mealId, int foodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{MealsBase}/{mealId}/foods/{foodId}", cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "RemoveFoodFromMeal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] RemoveFoodFromMeal exception: {ex.Message}");
                return false;
            }
        }

        // ── Liquids ───────────────────────────────────────────────────────────

        public async Task<List<LiquidDTO>> GetLiquidsByDiaryIdAsync(int diaryId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{LiquidsBase}/diary/{diaryId}", cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetLiquids"); return new List<LiquidDTO>(); }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<LiquidDTO>>>(_jsonOptions, cancellationToken);
                return result?.Data ?? new List<LiquidDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetLiquids exception: {ex.Message}");
                return new List<LiquidDTO>();
            }
        }

        public async Task<bool> CreateLiquidAsync(CreateLiquidDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(LiquidsBase, dto, _jsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateLiquid"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateLiquid exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateLiquidAsync(int liquidId, UpdateLiquidDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PutAsJsonAsync($"{LiquidsBase}/{liquidId}", dto, _jsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "UpdateLiquid"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] UpdateLiquid exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLiquidAsync(int liquidId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.DeleteAsync($"{LiquidsBase}/{liquidId}", cancellationToken);
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

        public async Task<List<DailyProgressDTO>> GetDailyProgressByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.GetAsync($"{ProgressBase}/user/{userId}", cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "GetDailyProgress"); return new List<DailyProgressDTO>(); }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<List<DailyProgressDTO>>>(_jsonOptions, cancellationToken);
                return result?.Data ?? new List<DailyProgressDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] GetDailyProgress exception: {ex.Message}");
                return new List<DailyProgressDTO>();
            }
        }

        public async Task<int?> CreateDailyProgressAsync(CreateDailyProgressDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync(ProgressBase, dto, _jsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "CreateDailyProgress"); return null; }
                var result = await response.Content.ReadFromJsonAsync<ApiSingleResponse<int>>(_jsonOptions, cancellationToken);
                return result?.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] CreateDailyProgress exception: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SetGoalAsync(int dailyProgressId, SetGoalDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("LifeSyncApi");
                var response = await client.PostAsJsonAsync($"{ProgressBase}/{dailyProgressId}/set-goal", dto, _jsonOptions, cancellationToken);
                if (!response.IsSuccessStatusCode) { await LogErrorAsync(response, "SetGoal"); return false; }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NutritionService] SetGoal exception: {ex.Message}");
                return false;
            }
        }
    }
}
