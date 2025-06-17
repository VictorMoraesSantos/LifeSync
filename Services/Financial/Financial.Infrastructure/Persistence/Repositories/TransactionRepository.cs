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
                .Include(t => t.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<Transaction?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<Transaction?> entities = await _context.Transactions
                .AsNoTracking()
                .Include(t => t.Category)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<Transaction?> GetById(int id, CancellationToken cancellationToken = default)
        {
            Transaction? entity = await _context.Transactions
                .Include(t => t.Category)
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
                .Include(t => t.Category)
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
