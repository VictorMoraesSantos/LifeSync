using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.Models.TaskManager;

namespace LifeSyncApp.Services.TaskManager
{
    public interface ITaskItemService
    {
        Task<TaskItem> GetTaskItemAsync(int id);
        Task<IEnumerable<TaskItem>> SearchTaskItemAsync(TaskItemFilterDTO query);
        Task<int> CreateTaskItemAsync(CreateTaskItemDTO data);
        Task UpdateTaskItemAsync(int id, UpdateTaskItemDTO data);
        Task DeleteTaskItemAsync(int id);
    }
}
