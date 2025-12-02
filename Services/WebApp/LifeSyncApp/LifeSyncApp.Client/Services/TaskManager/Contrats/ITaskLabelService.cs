using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.TaskManager.TaskLabel;

namespace LifeSyncApp.Client.Services.TaskManager.Contrats
{
    public interface ITaskLabelService
    {
        Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsAsync();
        Task<ApiResponse<int>> CreateLabelAsync(CreateTaskLabelDTO command);
        Task<ApiResponse<bool>> UpdateLabelAsync(UpdateTaskLabelDTO command);
        Task<ApiResponse<object>> DeleteLabelAsync(int id);
        Task<ApiResponse<List<TaskLabelDTO>>> SearchLabelsAsync(TaskLabelFilterDTO filter);
        Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsByUserAsync(int userId);
    }
}
