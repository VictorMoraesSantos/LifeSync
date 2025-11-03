using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.TaskManager;

namespace LifeSyncApp.Client.Services
{
    public interface ITaskManagerService
    {
        Task<HttpResult<List<TaskItemDto>>> GetTaskItemsAsync(int? userId = null);
        Task<HttpResult<TaskItemDto>> GetTaskItemByIdAsync(int id);
        Task<HttpResult<TaskItemDto>> CreateTaskItemAsync(CreateTaskItemRequest request);
        Task<HttpResult> UpdateTaskItemAsync(int id, UpdateTaskItemRequest request);
        Task<HttpResult> DeleteTaskItemAsync(int id);

        Task<HttpResult<List<TaskLabelDto>>> GetTaskLabelsAsync(int? userId = null);
        Task<HttpResult<TaskLabelDto>> GetTaskLabelByIdAsync(int id);
        Task<HttpResult<TaskLabelDto>> CreateTaskLabelAsync(CreateTaskLabelRequest request);
        Task<HttpResult> UpdateTaskLabelAsync(int id, UpdateTaskLabelRequest request);
        Task<HttpResult> DeleteTaskLabelAsync(int id);
    }
}
