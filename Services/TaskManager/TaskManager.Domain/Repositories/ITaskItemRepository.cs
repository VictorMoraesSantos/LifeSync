using Core.Domain.Repositories;
using TaskManager.Domain.Entities;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Repositories
{
    public interface ITaskItemRepository : IRepository<TaskItem, int>
    {
        public Task<(IEnumerable<TaskItem> Items, int TotalCount)> FindByFilter(TaskItemFilter filter, CancellationToken cancellationToken = default);
    }
}
