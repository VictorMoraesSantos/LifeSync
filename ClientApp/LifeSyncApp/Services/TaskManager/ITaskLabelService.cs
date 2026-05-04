using LifeSyncApp.Models.TaskManager;

namespace LifeSyncApp.Services.TaskManager
{
    public interface ITaskLabelService
    {
        Task<TaskLabel> GetTaskLabelAsync(int id);
        Task<IEnumerable<TaskLabel>> SearchTaskLabelAsync(TaskLabelFilterDTO query);
        Task<int> CreateTaskLabelAsync(CreateTaskLabelDTO data);
        Task EditTaskLabelAsync(int id, UpdateTaskLabelDTO data);
        Task DeleteTaskLabelAsync(int id);
    }
}
