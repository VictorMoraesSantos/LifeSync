using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.Nutrition;

namespace LifeSyncApp.Client.Services
{
    public interface INutritionService
    {
        Task<HttpResult<List<DiaryDto>>> GetDiariesAsync(int? userId = null);
        Task<HttpResult<DiaryDto>> GetDiaryByIdAsync(int id);
        Task<HttpResult<int>> CreateDiaryAsync(CreateDiaryRequest request); // <- alterado para int (Id)
        Task<HttpResult> UpdateDiaryAsync(int id, DateOnly date);
        Task<HttpResult> DeleteDiaryAsync(int id);

        Task<HttpResult<List<MealDto>>> GetMealsAsync(int? diaryId = null);
        Task<HttpResult<MealDto>> GetMealByIdAsync(int id);
        Task<HttpResult<MealDto>> CreateMealAsync(CreateMealRequest request);
        Task<HttpResult> UpdateMealAsync(int id, UpdateMealRequest request);
        Task<HttpResult> DeleteMealAsync(int id);
        Task<HttpResult<MealDto>> AddMealFoodAsync(int mealId, CreateMealFoodRequest request);
        Task<HttpResult> RemoveMealFoodAsync(int mealId, int foodId);

        Task<HttpResult<List<LiquidDto>>> GetLiquidsAsync(int? diaryId = null);
        Task<HttpResult<LiquidDto>> GetLiquidByIdAsync(int id);
        Task<HttpResult<LiquidDto>> CreateLiquidAsync(CreateLiquidRequest request);
        Task<HttpResult> UpdateLiquidAsync(int id, UpdateLiquidRequest request);
        Task<HttpResult> DeleteLiquidAsync(int id);

        Task<HttpResult<List<DailyProgressDto>>> GetDailyProgressesAsync(int? userId = null);
        Task<HttpResult<DailyProgressDto>> GetDailyProgressByIdAsync(int id);
        Task<HttpResult<DailyProgressDto>> CreateDailyProgressAsync(CreateDailyProgressRequest request);
        Task<HttpResult> UpdateDailyProgressAsync(int id, UpdateDailyProgressRequest request);
        Task<HttpResult> DeleteDailyProgressAsync(int id);
        Task<HttpResult> SetDailyProgressGoalAsync(int id, DailyGoalDto goal);
    }
}
