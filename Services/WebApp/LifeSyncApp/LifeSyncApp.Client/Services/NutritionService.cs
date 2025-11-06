using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Services.Http;
using System.Net.Http.Json;
using LifeSyncApp.Client.Models.Nutrition;

namespace LifeSyncApp.Client.Services
{
    public interface INutritionService
    {
        Task<ApiResponse<List<DiaryDTO>>> GetDiariesAsync();
        Task<ApiResponse<DiaryDTO>> GetDiaryByDateAsync(DateOnly date);
        Task<ApiResponse<int>> CreateDiaryAsync(CreateDiaryCommand command);
        Task<ApiResponse<bool>> UpdateDiaryAsync(UpdateDiaryCommand command);
        Task<ApiResponse<bool>> AddMealAsync(int diaryId, CreateMealCommand command);
        Task<ApiResponse<int>> AddLiquidAsync(CreateLiquidCommand command);

        Task<ApiResponse<List<DiaryDTO>>> SearchDiariesAsync(object filter);
        Task<ApiResponse<List<MealDTO>>> SearchMealsAsync(object filter);
        Task<ApiResponse<List<LiquidDTO>>> SearchLiquidsAsync(object filter);
        Task<ApiResponse<List<DiaryDTO>>> GetDiariesByUserAsync(int userId);
    }

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

        public async Task<ApiResponse<bool>> AddMealAsync(int diaryId, CreateMealCommand command)
        {
            try
            {
                var res = await _apiClient.PostAsync<CreateMealCommand, ApiResponse<bool>>($"nutrition-service/api/diaries/{diaryId}/meals", command);
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
    }
}
