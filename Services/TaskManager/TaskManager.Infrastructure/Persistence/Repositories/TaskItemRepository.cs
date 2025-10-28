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

            // Filtros específicos do TaskItem
            if (filter.UserId.HasValue)
                query = query.Where(t => t.UserId == filter.UserId.Value);

            if (!string.IsNullOrEmpty(filter.TitleContains))
                query = query.Where(t => t.Title.ToLower().Trim().Contains(filter.TitleContains.ToLower().Trim()));

            if (filter.LabelId.HasValue)
                query = query.Where(t => t.Labels.Any(l => l.Id == filter.LabelId.Value));

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.DueDate.HasValue)
                query = query.Where(t => t.DueDate == filter.DueDate.Value);

            // Filtros herdados do DomainQueryFilter
            if (filter.Id.HasValue)
                query = query.Where(t => t.Id == filter.Id.Value);

            if (filter.CreatedAt.HasValue)
                query = query.Where(t => t.CreatedAt.Date == filter.CreatedAt.Value.ToDateTime(TimeOnly.MinValue));

            if (filter.UpdatedAt.HasValue)
                query = query.Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date == filter.UpdatedAt.Value.ToDateTime(TimeOnly.MinValue));

            if (filter.IsDeleted.HasValue)
                query = query.Where(t => t.IsDeleted == filter.IsDeleted.Value);

            // Ordenação
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                query = filter.SortBy.ToLower() switch
                {
                    "id" => filter.SortDesc == true ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
                    "title" => filter.SortDesc == true ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                    "duedate" => filter.SortDesc == true ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                    "priority" => filter.SortDesc == true ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                    "status" => filter.SortDesc == true ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                    "createdat" => filter.SortDesc == true ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                    "updatedat" => filter.SortDesc == true ? query.OrderByDescending(t => t.UpdatedAt) : query.OrderBy(t => t.UpdatedAt),
                    _ => query.OrderBy(t => t.Id)
                };
            }
            else
            {
                query = query.OrderBy(t => t.Id); // Ordenação padrão
            }

            // Paginação
            if (filter.Page.HasValue && filter.PageSize.HasValue)
            {
                query = query
                    .Skip((filter.Page.Value - 1) * filter.PageSize.Value)
                    .Take(filter.PageSize.Value);
            }

            return await query.ToListAsync(cancellationToken);
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