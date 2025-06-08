using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Transaction entity, CancellationToken cancellationToken = default)
        {
            _context.Transactions.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Transaction> entities, CancellationToken cancellationToken = default)
        {
            _context.Transactions.AddRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Transaction entity, CancellationToken cancellationToken = default)
        {
            _context.Transactions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Transaction?>> Find(Expression<Func<Transaction, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<Transaction?> entities = await _context.Transactions
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<Transaction?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<Transaction?> entities = await _context.Transactions
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<Transaction?>> GetAllByAccountIdAsync(int accountId, int userId, DateTime startDate, DateTime endDate)
        {
            IEnumerable<Transaction?> entities = await _context.Transactions
                .Where(t => t.FinancialAccountId == accountId &&
                            t.UserId == userId &&
                            t.TransactionDate >= startDate &&
                            t.TransactionDate <= endDate)
                .AsNoTracking()
                .ToListAsync();

            return entities;
        }

        public async Task<Transaction?> GetById(int id, CancellationToken cancellationToken = default)
        {
            Transaction? entity = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<Transaction?>> GetByUserIdAsync(int userId, DateTime startDate, DateTime endDate, int? categoryId, TransactionType? type)
        {
            IEnumerable<Transaction?> entities = await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionDate >= startDate &&
                            t.TransactionDate <= endDate &&
                            (!categoryId.HasValue || t.CategoryId == categoryId))
                .AsNoTracking()
                .ToListAsync();

            return entities;
        }

        public async Task Update(Transaction entity, CancellationToken cancellationToken = default)
        {
            _context.Transactions.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
