using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.Nutrition;
using LifeSyncApp.Client.Services.Contracts;
using LifeSyncApp.Client.Services.Http;

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

        public async Task<ApiResponse<int>> AddMealFoodAsync(CreateMealFoodCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateMealFoodCommand, ApiResponse<int>>("nutrition-service/api/meal-foods", command);
                return res ?? new ApiResponse<int> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Success = false, Errors = new[] { ex.Message } };
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

        public async Task<ApiResponse<List<MealDTO>>> SearchMealsAsync(object filter)
        {
            try
            {
                var query = QueryStringHelper.ToQueryString(filter);
                var res = await _apiClient.GetAsync<ApiResponse<List<MealDTO>>>($"nutrition-service/api/meals/search{query}");
                return res ?? new ApiResponse<List<MealDTO>> { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<MealDTO>> { Success = false, Errors = new[] { ex.Message } };
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
                var res = await _apiClient.GetAsync<ApiResponse<List<DiaryDTO>>>($"nutrition-service/api/diaries/user/{userId}");
                return res ?? new ApiResponse<List<DiaryDTO>> { Success = false };
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
    }
}
