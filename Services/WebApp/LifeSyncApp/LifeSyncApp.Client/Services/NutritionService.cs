using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Services.Http;
using System.Net.Http.Json;

namespace LifeSyncApp.Client.Services
{
    // Nutrition Models
    public class NutritionDiary
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalCalories { get; set; }
        public decimal TotalLiquids { get; set; }
        public List<Meal> Meals { get; set; } = new();
    }

    public class Meal
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public decimal Calories { get; set; }
        public List<MealFood> Foods { get; set; } = new();
    }

    public class MealFood
    {
        public Guid Id { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "g";
        public decimal Calories { get; set; }
    }

    public class Liquid
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = "Water";
    }

    // Lightweight filter models that match API query parameter names
    public class DiaryFilter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? TotalCaloriesEquals { get; set; }
        public int? TotalCaloriesGreaterThan { get; set; }
        public int? TotalCaloriesLessThan { get; set; }
        public int? TotalLiquidsMlEquals { get; set; }
        public int? TotalLiquidsMlGreaterThan { get; set; }
        public int? TotalLiquidsMlLessThan { get; set; }
        public int? MealId { get; set; }
        public int? LiquidId { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class MealFilter
    {
        public int? Id { get; set; }
        public string? NameContains { get; set; }
        public string? DescriptionContains { get; set; }
        public int? DiaryId { get; set; }
        public int? TotalCaloriesEquals { get; set; }
        public int? TotalCaloriesGreaterThan { get; set; }
        public int? TotalCaloriesLessThan { get; set; }
        public int? MealFoodId { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class LiquidFilter
    {
        public int? Id { get; set; }
        public string? NameContains { get; set; }
        public int? QuantityMlEquals { get; set; }
        public int? QuantityMlGreaterThan { get; set; }
        public int? QuantityMlLessThan { get; set; }
        public int? CaloriesPerMlEquals { get; set; }
        public int? CaloriesPerMlGreaterThan { get; set; }
        public int? CaloriesPerMlLessThan { get; set; }
        public int? DiaryId { get; set; }
        public int? TotalCaloriesEquals { get; set; }
        public int? TotalCaloriesGreaterThan { get; set; }
        public int? TotalCaloriesLessThan { get; set; }
        public DateOnly? CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    // Nutrition Service
    public interface INutritionService
    {
        Task<ApiResponse<List<NutritionDiary>>> GetDiariesAsync();
        Task<ApiResponse<NutritionDiary>> GetDiaryByDateAsync(DateTime date);
        Task<ApiResponse<NutritionDiary>> CreateDiaryAsync(NutritionDiary diary);
        Task<ApiResponse<Meal>> AddMealAsync(Meal meal);
        Task<ApiResponse<Liquid>> AddLiquidAsync(Liquid liquid);

        // New search endpoints
        Task<ApiResponse<List<NutritionDiary>>> SearchDiariesAsync(DiaryFilter filter);
        Task<ApiResponse<List<Meal>>> SearchMealsAsync(MealFilter filter);
        Task<ApiResponse<List<Liquid>>> SearchLiquidsAsync(LiquidFilter filter);
        Task<ApiResponse<List<NutritionDiary>>> GetDiariesByUserAsync(int userId);
    }

    public class NutritionService : INutritionService
    {
        private readonly IApiClient _apiClient;

        public NutritionService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<NutritionDiary>>> GetDiariesAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<NutritionDiary>>>("nutrition-service/api/diaries");
                return res ?? new ApiResponse<List<NutritionDiary>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<NutritionDiary>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<NutritionDiary>> GetDiaryByDateAsync(DateTime date)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<NutritionDiary>>($"nutrition-service/api/diaries/date/{date:yyyy-MM-dd}");
                return res ?? new ApiResponse<NutritionDiary> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<NutritionDiary> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<NutritionDiary>> CreateDiaryAsync(NutritionDiary diary)
        {
            try
            {
                var res = await _apiClient.PostAsync<NutritionDiary, ApiResponse<NutritionDiary>>("nutrition-service/api/diaries", diary);
                return res ?? new ApiResponse<NutritionDiary> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<NutritionDiary> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<Meal>> AddMealAsync(Meal meal)
        {
            try
            {
                var res = await _apiClient.PostAsync<Meal, ApiResponse<Meal>>("nutrition-service/api/meals", meal);
                return res ?? new ApiResponse<Meal> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Meal> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<Liquid>> AddLiquidAsync(Liquid liquid)
        {
            try
            {
                var res = await _apiClient.PostAsync<Liquid, ApiResponse<Liquid>>("nutrition-service/api/liquids", liquid);
                return res ?? new ApiResponse<Liquid> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Liquid> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<NutritionDiary>>> SearchDiariesAsync(DiaryFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<NutritionDiary>>>($"nutrition-service/api/diaries/search{query}");
                return res ?? new ApiResponse<List<NutritionDiary>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<NutritionDiary>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Meal>>> SearchMealsAsync(MealFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<Meal>>>($"nutrition-service/api/meals/search{query}");
                return res ?? new ApiResponse<List<Meal>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Meal>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<Liquid>>> SearchLiquidsAsync(LiquidFilter filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<Liquid>>>($"nutrition-service/api/liquids/search{query}");
                return res ?? new ApiResponse<List<Liquid>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Liquid>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<NutritionDiary>>> GetDiariesByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<NutritionDiary>>>($"nutrition-service/api/diaries/user/{userId}");
                return res ?? new ApiResponse<List<NutritionDiary>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<NutritionDiary>> { Success = false, Errors = new[] { ex.Message } };
            }
        }
    }
}
