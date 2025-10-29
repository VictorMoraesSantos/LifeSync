using BuildingBlocks.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Filters;
using TaskManager.Domain.Repositories;
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

        public async Task<(IEnumerable<TaskLabel> Items, int TotalCount)> FindByFilter(TaskLabelFilter filter, CancellationToken cancellationToken = default)
        {
            IQueryable<TaskLabel> query = _context.TaskLabels
                .AsNoTracking()
                .AsQueryable();

            // Filtros específicos do TaskLabel
            query = query
                .ApplyFilter(filter.UserId, t => t.UserId)
                .ApplyFilter(filter.TaskItemId, t => (int)t.TaskItemId)
                .ApplyStringContains(filter.NameContains, t => t.Name)
                .ApplyFilter(filter.LabelColor, t => t.LabelColor)
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
