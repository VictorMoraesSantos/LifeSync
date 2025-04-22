using TaskManager.Application.DTOs;
using TaskManager.Application.TaskItems.Commands.CreateTaskItem;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskItemService
    {
        Task<TaskItemDTO> GetTaskItemByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<TaskItemDTO>> GetAllTaskItemsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<TaskItemDTO>> GetTaskItemsByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<TaskItemDTO>> GetTaskItemsByDueDateAsync(int userId, DateOnly dueDate, CancellationToken cancellationToken);
        Task<IEnumerable<TaskItemDTO>> GetTaskItemsByLabelIdAsync(int userId, int labelId, CancellationToken cancellationToken);
        Task<IEnumerable<TaskItemDTO>> GetTaskItemsByPriorityAsync(int userId, Priority priority, CancellationToken cancellationToken);
        Task<IEnumerable<TaskItemDTO>> GetTaskItemsByStatusAsync(int userId, Status status, CancellationToken cancellationToken);
        Task<IEnumerable<TaskItemDTO>> GetTaskItemsTitleAsync(int userId, string title, CancellationToken cancellationToken);
        Task<int> CreateTaskItemAsync(string title, string description, int priority, DateOnly dueDate, int userId, CancellationToken cancellationToken);
        Task<bool> UpdateTaskItemAsync(int id, string title, string description, int status, int priority, DateOnly dueDate, CancellationToken cancellationToken);
        Task<bool> DeleteTaskItemAsync(int id, CancellationToken cancellationToken);
    }
}
