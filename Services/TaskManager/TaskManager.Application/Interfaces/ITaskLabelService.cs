using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskLabelService
    {
        Task<TaskLabelDTO> GetTaskLabelByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<TaskLabelDTO>> GetAllTaskLabelsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<TaskLabelDTO>> GetTaskLabelsByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<TaskLabelDTO>> GetTaskLabelsByNameAsync(int userId, string name, CancellationToken cancellationToken);
        Task<IEnumerable<TaskLabelDTO>> GetTaskLabelsByTaskItemIdAsync(int userId, int taskItemId, CancellationToken cancellationToken);
        Task<int> CreateTaskLabelAsync(string name, int labelColor, int userId, int taskItemId, CancellationToken cancellationToken);
        Task<bool> UpdateTaskLabelAsync(int id, string name, int labelColor, int userId, int taskItemId, CancellationToken cancellationToken);
        Task<bool> DeleteTaskLabelAsync(int id, CancellationToken cancellationToken);
    }
}
