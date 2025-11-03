using LifeSyncApp.Client.Models.Dashboard;

namespace LifeSyncApp.Client.Services
{
    public interface IDashboardService
    {
        Task<TaskManagerDashboardDto> GetTaskManagerDashboardAsync(int userId);
        Task<NutritionDashboardDto> GetNutritionDashboardAsync(int userId);
        Task<FinancialDashboardDto> GetFinancialDashboardAsync(int userId);
        Task<GymDashboardDto> GetGymDashboardAsync(int userId);
    }
}
