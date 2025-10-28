using BuildingBlocks.Helpers;
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

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> FindByFilter(TaskItemFilter filter, CancellationToken cancellationToken = default)
        {

            IQueryable<TaskItem> query = _context.TaskItems
                .AsNoTracking()
                .Include(t => t.Labels)
                .AsQueryable();

            // Filtros específicos do TaskItem
            query = query
                .ApplyFilter(filter.UserId, t => t.UserId)
                .ApplyStringContains(filter.TitleContains, t => t.Title)
                .ApplyFilter(filter.Priority, t => t.Priority)
                .ApplyFilter(filter.Status, t => t.Status)
                .ApplyFilter(filter.DueDate, t => t.DueDate)
                .ApplyFilter(filter.Id, t => t.Id)
                .ApplyFilter(filter.IsDeleted, t => t.IsDeleted);

            var totalCount = await query.CountAsync(cancellationToken);

            // Ordenação
            query = string.IsNullOrEmpty(filter.SortBy) ? query.OrderBy(t => t.Id) : query.ApplyOrderBy(filter.SortBy, filter.SortDesc ?? false);

            // Paginação
            if (filter.Page.HasValue && filter.PageSize.HasValue)
            {
                query = query
                    .Skip((filter.Page.Value - 1) * filter.PageSize.Value)
                    .Take(filter.PageSize.Value);
            }

            var items = await query.ToListAsync(cancellationToken);

            return (items, totalCount);
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