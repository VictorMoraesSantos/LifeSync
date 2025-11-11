using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.Nutrition;
using LifeSyncApp.Client.Models.Nutrition.MealFood;

namespace LifeSyncApp.Client.Services.Contracts
{
    public interface INutritionService
    {
        Task<ApiResponse<List<DiaryDTO>>> GetDiariesAsync();
        Task<ApiResponse<DiaryDTO>> GetDiaryByDateAsync(DateOnly date);
        Task<ApiResponse<int>> CreateDiaryAsync(CreateDiaryCommand command);
        Task<ApiResponse<bool>> UpdateDiaryAsync(UpdateDiaryCommand command);
        Task<ApiResponse<bool>> AddMealAsync(CreateMealCommand command);
        Task<ApiResponse<int>> AddLiquidAsync(CreateLiquidCommand command);
        Task<ApiResponse<int>> AddMealFoodAsync(int mealId, CreateMealFoodCommand command);
        Task<ApiResponse<object>> DeleteDiaryAsync(int id);
        Task<ApiResponse<object>> DeleteMealAsync(int id);
        Task<ApiResponse<object>> DeleteLiquidAsync(int id);
        Task<ApiResponse<object>> DeleteMealFoodAsync(int id);

        Task<ApiResponse<List<DiaryDTO>>> SearchDiariesAsync(object filter);
        Task<ApiResponse<List<MealDTO>>> SearchMealsAsync(object filter);
        Task<ApiResponse<List<LiquidDTO>>> SearchLiquidsAsync(object filter);
        Task<ApiResponse<List<DiaryDTO>>> GetDiariesByUserAsync(int userId);

        // Daily progress
        Task<ApiResponse<List<DailyProgressDTO>>> GetDailyProgressesByUserAsync(int userId);
        Task<ApiResponse<int>> CreateDailyProgressAsync(CreateDailyProgressCommand command);
        Task<ApiResponse<bool>> UpdateDailyProgressAsync(UpdateDailyProgressCommand command);
        Task<ApiResponse<bool>> SetGoalAsync(int dailyProgressId, DailyGoalDTO goal);

        // Meal food update
        Task<ApiResponse<bool>> UpdateMealFoodAsync(int id, UpdateMealFoodRequest request);
    }
}
