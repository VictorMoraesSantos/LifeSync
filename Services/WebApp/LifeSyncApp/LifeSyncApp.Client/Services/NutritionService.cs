using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.Nutrition;
using LifeSyncApp.Client.Models.Nutrition.MealFood;
using LifeSyncApp.Client.Services.Contracts;
using LifeSyncApp.Client.Services.Http;
using FlatMealDTO = LifeSyncApp.Client.Models.Nutrition.MealDTO;

namespace LifeSyncApp.Client.Services
{
    public class NutritionService : INutritionService
    {
        private readonly IApiClient _apiClient;

        public NutritionService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiResponse<List<DiaryDTO>>> GetDiariesAsync()
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<DiaryDTO>>>("nutrition-service/api/diaries");
                return res ?? new ApiResponse<List<DiaryDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<DiaryDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<DiaryDTO>> GetDiaryByDateAsync(DateOnly date)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<DiaryDTO>>($"nutrition-service/api/diaries/date/{date:yyyy-MM-dd}");
                return res ?? new ApiResponse<DiaryDTO> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<DiaryDTO> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateDiaryAsync(CreateDiaryCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateDiaryCommand, ApiResponse<int>>("nutrition-service/api/diaries", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateDiaryAsync(UpdateDiaryCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateDiaryCommand, ApiResponse<bool>>($"nutrition-service/api/diaries/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> AddMealAsync(CreateMealCommand command)
        {
            try
            {
                // MealsController -> POST api/meals
                var res = await _apiClient.PostAsync<CreateMealCommand, ApiResponse<bool>>("nutrition-service/api/meals", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> AddLiquidAsync(CreateLiquidCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateLiquidCommand, ApiResponse<int>>("nutrition-service/api/liquids", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> AddMealFoodAsync(int mealId, CreateMealFoodCommand command)
        {
            try
            {
                // Endpoint: POST api/meals/{mealId}/foods
                var res = await _apiClient.PostAsync<CreateMealFoodCommand, ApiResponse<int>>($"nutrition-service/api/meals/{mealId}/foods", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteDiaryAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"nutrition-service/api/diaries/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteMealAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"nutrition-service/api/meals/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteLiquidAsync(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"nutrition-service/api/liquids/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<object>> DeleteMealFoodAsync(int id)
        {
            try
            {
                // Endpoint: DELETE api/meal-foods/{id}
                await _apiClient.DeleteAsync($"nutrition-service/api/meal-foods/{id}");
                return new ApiResponse<object> { Success = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<DiaryDTO>>> SearchDiariesAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<DiaryDTO>>>($"nutrition-service/api/diaries/search{query}");
                return res ?? new ApiResponse<List<DiaryDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<DiaryDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<FlatMealDTO>>> SearchMealsAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<FlatMealDTO>>>($"nutrition-service/api/meals/search{query}");
                return res ?? new ApiResponse<List<FlatMealDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<FlatMealDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<LiquidDTO>>> SearchLiquidsAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<LiquidDTO>>>($"nutrition-service/api/liquids/search{query}");
                return res ?? new ApiResponse<List<LiquidDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<LiquidDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<List<DiaryDTO>>> GetDiariesByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<DiaryDTO>>>($"nutrition-service/api/diaries/user/{userId}")
                    ?? new ApiResponse<List<DiaryDTO>> { Success = false };
                return res;
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<DiaryDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        // Daily progress
        public async Task<ApiResponse<List<DailyProgressDTO>>> GetDailyProgressesByUserAsync(int userId)
        {
            try
            {
                var res = await _apiClient.GetAsync<ApiResponse<List<DailyProgressDTO>>>($"nutrition-service/api/daily-progresses/user/{userId}");
                return res ?? new ApiResponse<List<DailyProgressDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<DailyProgressDTO>> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<int>> CreateDailyProgressAsync(CreateDailyProgressCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateDailyProgressCommand, ApiResponse<int>>("nutrition-service/api/daily-progresses", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateDailyProgressAsync(UpdateDailyProgressCommand command)
        {
            try
            {
                var res = await _apiClient.PutAsync<UpdateDailyProgressCommand, ApiResponse<bool>>($"nutrition-service/api/daily-progresses/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> SetGoalAsync(int dailyProgressId, DailyGoalDTO goal)
        {
            try
            {
                var payload = new SetGoalRequest(goal);
                var res = await _apiClient.PostAsync<SetGoalRequest, ApiResponse<bool>>($"nutrition-service/api/daily-progresses/{dailyProgressId}/set-goal", payload);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateMealFoodAsync(int id, UpdateMealFoodRequest request)
        {
            try
            {
                // Endpoint: PUT api/meal-foods/{id}
                var res = await _apiClient.PutAsync<UpdateMealFoodRequest, ApiResponse<bool>>($"nutrition-service/api/meal-foods/{id}", request);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        public async Task<ApiResponse<bool>> UpdateMealAsync(LifeSyncApp.Client.Models.Nutrition.Meal.UpdateMealDTO command)
        {
            try
            {
                var res = await _apiClient.PutAsync<LifeSyncApp.Client.Models.Nutrition.Meal.UpdateMealDTO, ApiResponse<bool>>($"nutrition-service/api/meals/{command.Id}", command);
                return res ?? new ApiResponse<bool> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Errors = new[] { ex.Message } };
            }
        }

        // Expose raw send for command objects if needed
        public async Task<HttpResponseMessage> SendRawAsync(HttpRequestMessage request)
        {
            return await _apiClient.SendAsync(request);
        }
    }
}
