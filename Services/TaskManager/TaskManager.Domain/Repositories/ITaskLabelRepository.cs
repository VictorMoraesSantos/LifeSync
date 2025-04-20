using Core.Domain.Repositories;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Repositories
{
    public interface ITaskLabelRepository : IRepository<TaskLabel, int>
    {
        Task<IEnumerable<TaskLabel?>> GetByName(int userId, string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskLabel?>> GetByUserId(int userId, CancellationToken cancellationToken = default);
    }
}
