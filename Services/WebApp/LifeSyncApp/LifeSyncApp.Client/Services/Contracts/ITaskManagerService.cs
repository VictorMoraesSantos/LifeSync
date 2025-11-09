using LifeSyncApp.Client.Models;
using LifeSyncApp.Client.Models.TaskManager;
using LifeSyncApp.Client.Models.TaskManager.TaskItem;
using LifeSyncApp.Client.Models.TaskManager.TaskLabel;

namespace LifeSyncApp.Client.Services.Contracts
{
    public interface ITaskManagerService
    {
        // TaskItems
        Task<ApiResponse<List<TaskItemDTO>>> GetTasksAsync();
        Task<ApiResponse<TaskItemDTO>> GetTaskByIdAsync(int id);
        Task<ApiResponse<int>> CreateTaskAsync(CreateTaskItemCommand command);
        Task<ApiResponse<bool>> UpdateTaskAsync(UpdateTaskItemCommand command);
        Task<ApiResponse<object>> DeleteTaskAsync(int id);

        // Labels
        Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsAsync();
        Task<ApiResponse<int>> CreateLabelAsync(CreateTaskLabelCommand command);
        Task<ApiResponse<bool>> UpdateLabelAsync(UpdateTaskLabelCommand command);
        Task<ApiResponse<object>> DeleteLabelAsync(int id);

        // Search and by-user
        Task<ApiResponse<List<TaskItemDTO>>> SearchTasksAsync(TaskItemFilterDTO filter);
        Task<ApiResponse<List<TaskLabelDTO>>> SearchLabelsAsync(TaskLabelFilterDTO filter);
        Task<ApiResponse<List<TaskItemDTO>>> GetTasksByUserAsync(int userId);
        Task<ApiResponse<List<TaskLabelDTO>>> GetLabelsByUserAsync(int userId);
    }
}
