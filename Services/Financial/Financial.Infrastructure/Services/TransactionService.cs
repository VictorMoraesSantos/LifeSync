using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using Financial.Domain.Errors;
using Financial.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<TransactionDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _transactionRepository.GetAll(cancellationToken);

                return Result.Success(all.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar transações");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateTransactionDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(TransactionErrors.CreateError);

                var entity = TransactionMapper.ToEntity(dto);

                await _transactionRepository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar transação {@TransactionData}", dto);
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateTransactionDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.NullValue);

                var entities = dtos.Select(TransactionMapper.ToEntity).ToList();

                await _transactionRepository.CreateRange(entities, cancellationToken);

                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplas transações");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(TransactionErrors.InvalidId);

                var entity = await _transactionRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(TransactionErrors.NotFound(id));

                await _transactionRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir transação {TransactionId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.NullValue);

                var entities = new List<Transaction>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _transactionRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                    return Result.Failure<bool>(Error.NullValue);

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NullValue);

                foreach (var entity in entities)
                    await _transactionRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplas transações {TransactionIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TransactionDTO>>> FindAsync(Expression<Func<TransactionDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<TransactionDTO>>(Error.NullValue);

                var all = await _transactionRepository.GetAll(cancellationToken);
                var dtos = all
                    .Select(TransactionMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<TransactionDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações com predicado");
                return Result.Failure<IEnumerable<TransactionDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TransactionDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _transactionRepository.GetAll(cancellationToken);
                var dtos = all.Select(TransactionMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<TransactionDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as transações");
                return Result.Failure<IEnumerable<TransactionDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<TransactionDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<TransactionDTO>(TransactionErrors.InvalidId);

                var entity = await _transactionRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<TransactionDTO>(TransactionErrors.NotFound(id));

                var dto = TransactionMapper.ToDTO(entity);

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transação {TransactionId}", id);
                return Result.Failure<TransactionDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TransactionDTO>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                    return Result.Failure<IEnumerable<TransactionDTO>>(TransactionErrors.InvalidId);

                var all = await _transactionRepository.Find(t => t.UserId == userId, cancellationToken);
                var dtos = all.Select(TransactionMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<TransactionDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações do usuário {UserId}", userId);
                return Result.Failure<IEnumerable<TransactionDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<TransactionDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1)
                    return Result.Failure<(IEnumerable<TransactionDTO>, int)>(Error.Failure("Página deve ser maior que zero"));

                if (pageSize < 1)
                    return Result.Failure<(IEnumerable<TransactionDTO>, int)>(Error.Failure("Tamanho da página deve ser maior que zero"));

                var all = await _transactionRepository.GetAll(cancellationToken);
                var totalCount = all.Count();
                var items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(TransactionMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<TransactionDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de transações (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<TransactionDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateTransactionDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                if (dto.Id <= 0)
                    return Result.Failure<bool>(TransactionErrors.InvalidId);

                var entity = await _transactionRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(TransactionErrors.NotFound(dto.Id));

                entity.Update(
                    dto.CategoryId,
                    dto.PaymentMethod,
                    dto.TransactionType,
                    dto.Amount,
                    dto.Description,
                    dto.TransactionDate,
                    dto.IsRecurring);

                await _transactionRepository.Update(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar transação {@TransactionData}", dto);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}