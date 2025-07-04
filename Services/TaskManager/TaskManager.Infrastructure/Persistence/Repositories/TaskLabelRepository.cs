using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.ValueObjects;
using TaskManager.Infrastructure.Persistence.Data;

namespace TaskManager.Infrastructure.Persistence.Repositories
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
            TaskLabel? entity = await _context.TaskLabels
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<TaskLabel?>> FindByFilter(TaskLabelFilter filter, CancellationToken cancellationToken = default)
        {
            IQueryable<TaskLabel> query = _context.TaskLabels
                .AsNoTracking()
                .AsQueryable();

            if (filter.UserId.HasValue)
                query = query.Where(x => x.UserId == filter.UserId.Value);

            if (filter.TaskItemId.HasValue)
                query = query.Where(x => x.TaskItemId == filter.TaskItemId.Value);

            if (!string.IsNullOrWhiteSpace(filter.NameContains))
                query = query.Where(x => x.Name.Contains(filter.NameContains));

            if (filter.LabelColor.HasValue)
                query = query.Where(x => x.LabelColor == filter.LabelColor.Value);

            IEnumerable<TaskLabel> entities = await query.ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<IEnumerable<TaskLabel?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel> entities = await _context.TaskLabels
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<TaskLabel?>> Find(Expression<Func<TaskLabel, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel> entities = await _context.TaskLabels
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Create(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            await _context.TaskLabels.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<TaskLabel> entities, CancellationToken cancellationToken = default)
        {
            await _context.TaskLabels.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            _context.TaskLabels.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            _context.TaskLabels.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
