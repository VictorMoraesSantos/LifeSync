using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using Financial.Domain.Repositories;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<int> CountAsync(Expression<Func<TransactionDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _transactionRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<int> CreateAsync(CreateTransactionDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var entity = TransactionMapper.ToEntity(dto);
            await _transactionRepository.Create(entity, cancellationToken);
            return entity.Id;
        }

        public async Task<IEnumerable<int>> CreateRangeAsync(IEnumerable<CreateTransactionDTO> dto, CancellationToken cancellationToken = default)
        {
            if (dto == null || !dto.Any()) throw new ArgumentNullException(nameof(dto));
            var entities = dto.Select(TransactionMapper.ToEntity).ToList();
            await _transactionRepository.CreateRange(entities, cancellationToken);
            return entities.Select(e => e.Id);
        }

        public async Task<bool> DeleteAsync(int dto, CancellationToken cancellationToken = default)
        {
            if (dto <= 0) throw new ArgumentOutOfRangeException(nameof(dto), "ID must be greater than zero.");
            var entity = await _transactionRepository.GetById(dto, cancellationToken);
            if (entity == null) return false;
            await _transactionRepository.Delete(entity, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> dtos, CancellationToken cancellationToken = default)
        {
            if (dtos == null || !dtos.Any()) return false;
            List<Transaction> entities = new();
            foreach (var id in dtos)
            {
                Transaction? entity = await _transactionRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }
            if (!entities.Any()) return false;
            foreach (var entity in entities)
                await _transactionRepository.Delete(entity, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<TransactionDTO?>> FindAsync(Expression<Func<TransactionDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var all = await _transactionRepository.GetAll(cancellationToken);
            var allDtos = all.Select(TransactionMapper.ToDTO).AsQueryable();
            var filtered = allDtos.Where(predicate).ToList();
            return filtered;
        }

        public async Task<IEnumerable<TransactionDTO?>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var all = await _transactionRepository.GetAll(cancellationToken);
            var dtos = all.Select(TransactionMapper.ToDTO).ToList();
            return dtos;

        }

        public async Task<TransactionDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");

            var entity = await _transactionRepository.GetById(id, cancellationToken);
            if (entity == null) return null;

            var dto = TransactionMapper.ToDTO(entity);

            return dto;
        }

        public async Task<IEnumerable<TransactionDTO>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than zero.");
            var all = await _transactionRepository.Find(t => t.UserId == userId, cancellationToken);
            var dtos = all.Select(TransactionMapper.ToDTO).ToList();
            return dtos;
        }

        public async Task<(IEnumerable<TransactionDTO?> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            var all = await _transactionRepository.GetAll(cancellationToken);
            var totalCount = all.Count();
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).Select(TransactionMapper.ToDTO).ToList();
            return (items, totalCount);
        }

        public async Task<bool> UpdateAsync(UpdateTransactionDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Id <= 0) throw new ArgumentOutOfRangeException(nameof(dto.Id), "ID must be greater than zero.");

            var entity = await _transactionRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            entity.Update(dto.Type, dto.Amount, dto.Description, dto.TransactionDate, dto.CategoryId, dto.IsRecurring);

            await _transactionRepository.Update(entity, cancellationToken);

            return true;
        }
    }
}
