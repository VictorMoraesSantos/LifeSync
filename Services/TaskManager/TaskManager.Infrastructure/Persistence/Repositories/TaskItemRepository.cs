using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.ValueObjects;
using TaskManager.Infrastructure.Persistence.Data;

namespace TaskManager.Infrastructure.Persistence.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem?> GetById(int id, CancellationToken cancellationToken = default)
        {
            TaskItem? entity = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<TaskItem?>> FindByFilter(TaskItemFilter filter, CancellationToken cancellationToken = default)
        {
            IQueryable<TaskItem> query = _context.TaskItems
                .AsNoTracking()
                .Include(t => t.Labels)
                .AsQueryable();

            if (filter.UserId.HasValue)
                query = query.Where(t => t.UserId == filter.UserId.Value);

            if (!string.IsNullOrEmpty(filter.TitleContains))
                query = query.Where(t => t.Title.Contains(filter.TitleContains));

            if (filter.LabelId.HasValue)
                query = query.Where(t => t.Labels.Any(l => l.Id == filter.LabelId.Value));

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.DueDate.HasValue)
                query = query.Where(t => t.DueDate == filter.DueDate.Value);

            IEnumerable<TaskItem> entities = await query.ToListAsync(cancellationToken);
            return entities;
        }

        public async Task<IEnumerable<TaskItem?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> entities = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<TaskItem?>> Find(Expression<Func<TaskItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> entities = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Create(TaskItem entity, CancellationToken cancellationToken = default)
        {
            await _context.TaskItems.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<TaskItem> entities, CancellationToken cancellationToken = default)
        {
            await _context.TaskItems.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(TaskItem entity, CancellationToken cancellationToken = default)
        {
            _context.TaskItems.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TaskItem entity, CancellationToken cancellationToken = default)
        {
            _context.TaskItems.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}