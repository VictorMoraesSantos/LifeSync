using LifeSyncApp.Models.Nutrition.DailyProgress;
using LifeSyncApp.Models.Nutrition.Diary;
using LifeSyncApp.Models.Nutrition.Food;
using LifeSyncApp.Models.Nutrition.Liquid;
using LifeSyncApp.Models.Nutrition.LiquidType;
using LifeSyncApp.Models.Nutrition.Meal;
using LifeSyncApp.Models.Nutrition.MealFood;

namespace LifeSyncApp.Services.Nutrition
{
    public interface INutritionService
    {
        Task<List<DiaryDTO>> GetDiariesByUserIdAsync(int userId, CancellationToken ct = default);
        Task<DiaryDTO?> GetDiaryByIdAsync(int id, CancellationToken ct = default);
        Task<DiaryDTO?> GetDiaryByDateAsync(int userId, DateOnly date, CancellationToken ct = default);
        Task<(int? Id, string? Error)> CreateDiaryAsync(CreateDiaryDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> UpdateDiaryAsync(int id, UpdateDiaryDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> DeleteDiaryAsync(int id, CancellationToken ct = default);
        Task<List<DiaryDTO>> SearchDiariesAsync(int userId, DateOnly? dateFrom = null, DateOnly? dateTo = null, int? page = null, int? pageSize = null, CancellationToken ct = default);
        Task<List<FoodDTO>> SearchFoodsAsync(string? nameContains = null, int? page = null, int? pageSize = null, CancellationToken ct = default);
        Task<List<MealDTO>> GetMealsByDiaryIdAsync(int diaryId, CancellationToken ct = default);
        Task<MealDTO?> GetMealByIdAsync(int id, CancellationToken ct = default);
        Task<(bool Success, string? Error)> CreateMealAsync(CreateMealDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> UpdateMealAsync(int mealId, UpdateMealDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> DeleteMealAsync(int mealId, CancellationToken ct = default);
        Task<(bool Success, string? Error)> AddFoodToMealAsync(int mealId, CreateMealFoodDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> RemoveFoodFromMealAsync(int mealId, int foodId, CancellationToken ct = default);
        Task<(bool Success, string? Error)> UpdateMealFoodAsync(int id, UpdateMealFoodDTO dto, CancellationToken ct = default);
        Task<List<LiquidTypeDTO>> GetLiquidTypesAsync(CancellationToken ct = default);
        Task<List<LiquidDTO>> GetLiquidsByDiaryIdAsync(int diaryId, CancellationToken ct = default);
        Task<(bool Success, string? Error)> CreateLiquidAsync(CreateLiquidDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> UpdateLiquidAsync(int liquidId, UpdateLiquidDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> DeleteLiquidAsync(int liquidId, CancellationToken ct = default);
        Task<List<DailyProgressDTO>> GetDailyProgressByUserIdAsync(int userId, CancellationToken ct = default);
        Task<int?> CreateDailyProgressAsync(CreateDailyProgressDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> SetGoalAsync(int dailyProgressId, SetGoalDTO dto, CancellationToken ct = default);
        Task<(bool Success, string? Error)> UpdateDailyProgressAsync(int id, UpdateDailyProgressDTO dto, CancellationToken ct = default);
        void InvalidateAllCache();
    }
}
