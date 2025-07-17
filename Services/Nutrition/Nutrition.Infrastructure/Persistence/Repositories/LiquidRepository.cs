using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Persistence.Repositories
{
    public class LiquidRepository : ILiquidRepository
    {
        private readonly ApplicationDbContext _context;

        public LiquidRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Liquid?> GetById(int id, CancellationToken cancellationToken = default)
        {
            Liquid? liquid = await _context.Liquids
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            return liquid;
        }

        public async Task<IEnumerable<Liquid?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<Liquid?> liquids = await _context.Liquids
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return liquids;
        }

        public async Task<IEnumerable<Liquid?>> Find(Expression<Func<Liquid, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<Liquid?> liquids = await _context.Liquids
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();

            return liquids;
        }

        public async Task Create(Liquid entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Liquid> entities, CancellationToken cancellationToken = default)
        {
            _context.Liquids.AddRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(Liquid entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Liquid entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}