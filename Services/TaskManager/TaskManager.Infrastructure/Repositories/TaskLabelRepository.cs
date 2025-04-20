using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskLabelRepository : ITaskLabelRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskLabelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskLabel?> GetById(int id, CancellationToken cancellationToken = default)
        {
            TaskLabel? taskLabel = await _context.TaskLabels
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            return taskLabel;
        }

        public async Task<IEnumerable<TaskLabel?>> GetByUserId(int userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel> taskLabels = await _context.TaskLabels
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskLabels;
        }

        public async Task<IEnumerable<TaskLabel?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel> taskLabels = await _context.TaskLabels
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskLabels;
        }

        public async Task<IEnumerable<TaskLabel?>> Find(Expression<Func<TaskLabel, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel> taskLabels = await _context.TaskLabels
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return taskLabels;
        }

        public async Task<IEnumerable<TaskLabel?>> GetByName(int userId, string name, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel> taskLabels = await _context.TaskLabels
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.Name.Contains(name) && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskLabels;
        }

        public async Task Create(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            _context.TaskLabels.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<TaskLabel> entities, CancellationToken cancellationToken = default)
        {
            await _context.TaskLabels.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            _context.TaskLabels.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            _context.TaskLabels.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
