using Core.Domain.Repositories;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Filters;

namespace TaskManager.Domain.Repositories
{
    public interface ITaskLabelRepository : IRepository<TaskLabel, int, TaskLabelFilter>
    { }
}
