using Core.Domain.Repositories;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Filters;

namespace TaskManager.Domain.Repositories
{
    public interface ITaskItemRepository : IRepository<TaskItem, int, TaskItemFilter>
    {
    }
}
