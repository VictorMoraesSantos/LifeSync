using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.TaskManager.TaskItem;

namespace LifeSyncApp.Client.Services.TaskManager.Contrats
{
    public interface ITaskItemService
    {
        Task<ApiResponse<List<TaskItemDTO>>> GetTasksAsync();
        Task<ApiResponse<TaskItemDTO>> GetTaskByIdAsync(int id);
        Task<ApiResponse<int>> CreateTaskAsync(CreateTaskItemDTO command);
        Task<ApiResponse<bool>> UpdateTaskAsync(UpdateTaskItemDTO command);
        Task<ApiResponse<object>> DeleteTaskAsync(int id);
        Task<ApiResponse<List<TaskItemDTO>>> SearchTasksAsync(TaskItemFilterDTO filter);
        Task<ApiResponse<List<TaskItemDTO>>> GetTasksByUserAsync(int userId);
    }
}
