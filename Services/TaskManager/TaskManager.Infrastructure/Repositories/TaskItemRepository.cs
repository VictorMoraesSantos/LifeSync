using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
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
            TaskItem? taskItem = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            return taskItem;
        }

        public async Task<IEnumerable<TaskItem?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> taskItems = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskItems;
        }

        public async Task<IEnumerable<TaskItem?>> Find(Expression<Func<TaskItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> taskItems = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(x => !x.IsDeleted)
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return taskItems;
        }

        public async Task<IEnumerable<TaskItem?>> GetByUserId(int userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> taskItems = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskItems;
        }

        public async Task<IEnumerable<TaskItem>> GetByLabel(int userId, int labelId, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> taskItems = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(x => x.UserId == userId
                    && x.Labels.Any(l => l.Id == labelId)
                    && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskItems;
        }

        public async Task<IEnumerable<TaskItem?>> GetByPriority(int userId, Priority priority, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> taskItems = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(x => x.UserId == userId && x.Priority == priority && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskItems;
        }

        public async Task<IEnumerable<TaskItem?>> GetByStatus(int userId, Status status, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> taskItems = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(x => x.UserId == userId && x.Status == status && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskItems;
        }

        public async Task<IEnumerable<TaskItem?>> GetByDueDate(int userId, DateOnly dueDate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> taskItems = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(x => x.UserId == userId && x.DueDate == dueDate && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskItems;
        }

        public async Task<IEnumerable<TaskItem?>> GetTitleContains(int userId, string title, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> taskItems = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(x => x.UserId == userId && x.Title.Contains(title) && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return taskItems;
        }

        public async Task Create(TaskItem entity, CancellationToken cancellationToken = default)
        {
            _context.TaskItems.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<TaskItem> entities, CancellationToken cancellationToken = default)
        {
            await _context.TaskItems.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(TaskItem entity, CancellationToken cancellationToken = default)
        {
            _context.TaskItems.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TaskItem entity, CancellationToken cancellationToken = default)
        {
            _context.TaskItems.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}