using Core.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Filters;
using Nutrition.Domain.Filters.Specifications;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Persistence.Repositories
{
    public class LiquidTypeRepository : ILiquidTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public LiquidTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LiquidType?> GetById(int id, CancellationToken cancellationToken = default)
        {
            LiquidType? liquidType = await _context.LiquidTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(lt => lt.Id == id, cancellationToken);

            return liquidType;
        }

        public async Task<IEnumerable<LiquidType?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<LiquidType?> liquidTypes = await _context.LiquidTypes
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return liquidTypes;
        }

        public async Task<IEnumerable<LiquidType?>> Find(Expression<Func<LiquidType, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<LiquidType?> liquidTypes = await _context.LiquidTypes
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();

            return liquidTypes;
        }

        public async Task Create(LiquidType entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<LiquidType> entities, CancellationToken cancellationToken = default)
        {
            _context.LiquidTypes.AddRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(LiquidType entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(LiquidType entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IEnumerable<LiquidType> Items, int TotalCount)> FindByFilter(LiquidTypeQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new LiquidTypeSpecification(filter);
            IQueryable<LiquidType> query = _context.LiquidTypes.AsNoTracking();
            IQueryable<LiquidType> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<LiquidType> finalQuery = SpecificationEvaluator.GetQuery(_context.LiquidTypes.AsNoTracking(), spec);
            IEnumerable<LiquidType> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
