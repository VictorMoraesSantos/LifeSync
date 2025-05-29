using Financial.Domain.Entities;
using Financial.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Persistence.Repositories
{
    public class FinancialAccountRepository : IFinancialAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public FinancialAccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(FinancialAccount entity, CancellationToken cancellationToken = default)
        {
            _context.FinancialAccounts.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<FinancialAccount> entities, CancellationToken cancellationToken = default)
        {
            _context.FinancialAccounts.AddRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(FinancialAccount entity, CancellationToken cancellationToken = default)
        {
            _context.FinancialAccounts.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<FinancialAccount?>> Find(Expression<Func<FinancialAccount, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<FinancialAccount?> entities = await _context.FinancialAccounts
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<FinancialAccount?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<FinancialAccount?> entities = await _context.FinancialAccounts
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<FinancialAccount>> GetAllByUserIdAsync(int userId)
        {
            IEnumerable<FinancialAccount> entities = await _context.FinancialAccounts
                .Where(fa => fa.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return entities;
        }

        public async Task<FinancialAccount?> GetById(int id, CancellationToken cancellationToken = default)
        {
            FinancialAccount? entity = await _context.FinancialAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(fa => fa.Id == id, cancellationToken);

            return entity;
        }

        public async Task Update(FinancialAccount entity, CancellationToken cancellationToken = default)
        {
            _context.FinancialAccounts.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
