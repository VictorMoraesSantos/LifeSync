using Core.Domain.Repositories;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Repositories
{
    public interface ITaskItemRepository : IRepository<TaskItem, int>
    {
        Task<IEnumerable<TaskItem?>> GetByUserId(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem?>> GetTitleContains(int userId, string title, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem?>> GetByPriority(int userId, Priority priority, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem?>> GetByStatus(int userId, Status status, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem?>> GetByLabel(int userId, TaskLabel label, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem?>> GetByDueDate(int userId, DateOnly dueDate, CancellationToken cancellationToken = default);
    }
}
