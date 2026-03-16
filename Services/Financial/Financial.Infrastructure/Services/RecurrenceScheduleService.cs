using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using Financial.Domain.Errors;
using Financial.Domain.Filters;
using Financial.Domain.Repositories;
using Financial.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Services
{
    public class RecurrenceScheduleService : IRecurrenceScheduleService
    {
        private readonly IRecurrenceScheduleRepository _scheduleRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RecurrenceScheduleService> _logger;

        public RecurrenceScheduleService(
            IRecurrenceScheduleRepository scheduleRepository,
            ITransactionRepository transactionRepository,
            ApplicationDbContext dbContext,
            ILogger<RecurrenceScheduleService> logger)
        {
            _scheduleRepository = scheduleRepository;
            _transactionRepository = transactionRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<bool>> ActiveScheduleAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _scheduleRepository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(RecurrenceScheduleErrors.NotFound(id));

                entity.Activate();

                await _scheduleRepository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar agendamento de recorrência com ID {ScheduleId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<RecurrenceScheduleDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _scheduleRepository.GetAll(cancellationToken);
                var dtos = all.Select(RecurrencyScheduleMapper.ToDTO);

                var total = predicate != null
                    ? dtos.AsQueryable().Where(predicate).Count()
                    : dtos.Count();

                return Result.Success(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar agendamentos de recorrência");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateRecurrenceScheduleDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto is null) return Result.Failure<int>(Error.NullValue);

                var transaction = await _transactionRepository.GetById(dto.TransactionId, cancellationToken);
                if (transaction == null)
                    return Result.Failure<int>(RecurrenceScheduleErrors.InvalidTransaction);

                if (!transaction.IsRecurring)
                    return Result.Failure<int>(RecurrenceScheduleErrors.TransactionNotRecurring);

                var existing = await _scheduleRepository.GetByTransactionId(dto.TransactionId, cancellationToken);
                if (existing != null)
                    return Result.Failure<int>(RecurrenceScheduleErrors.ScheduleAlreadyExists);

                var entity = RecurrencyScheduleMapper.ToEntity(dto);
                await _scheduleRepository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar agendador para transação {TransactionId}", dto.TransactionId);
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateRecurrenceScheduleDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos is null || !dtos.Any()) return Result.Failure<IEnumerable<int>>(Error.NullValue);

                var entities = new List<RecurrenceSchedule>();

                foreach (var dto in dtos)
                {
                    var transaction = await _transactionRepository.GetById(dto.TransactionId, cancellationToken);
                    if (transaction == null)
                        return Result.Failure<IEnumerable<int>>(RecurrenceScheduleErrors.InvalidTransaction);

                    if (!transaction.IsRecurring)
                        return Result.Failure<IEnumerable<int>>(RecurrenceScheduleErrors.TransactionNotRecurring);

                    var existing = await _scheduleRepository.GetByTransactionId(dto.TransactionId, cancellationToken);
                    if (existing != null)
                        return Result.Failure<IEnumerable<int>>(RecurrenceScheduleErrors.ScheduleAlreadyExists);

                    entities.Add(RecurrencyScheduleMapper.ToEntity(dto));
                }

                await _scheduleRepository.CreateRange(entities, cancellationToken);
                return Result.Success(entities.Select(e => e.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar agendadores de recorrência em lote");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeactiveScheduleAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _scheduleRepository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(RecurrenceScheduleErrors.NotFound(id));

                entity.Deactivate();

                await _scheduleRepository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar agendamento de recorrência com ID {ScheduleId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0) return Result.Failure<bool>(RecurrenceScheduleErrors.InvalidId);

                var entity = await _scheduleRepository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<bool>(RecurrenceScheduleErrors.NotFound(id));

                await _scheduleRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar agendamento de recorrência com ID {ScheduleId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids is null || !ids.Any()) return Result.Failure<bool>(Error.NullValue);
                var entities = new List<RecurrenceSchedule>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _scheduleRepository.GetById(id, cancellationToken);
                    if (entity is not null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any()) return Result.Failure<bool>(Error.NullValue);
                if (!entities.Any()) return Result.Failure<bool>(Error.NullValue);

                foreach (var entity in entities)
                    await _scheduleRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar agendamento de recorrência para IDs {ScheduleIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RecurrenceScheduleDTO?>>> FindAsync(Expression<Func<RecurrenceScheduleDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _scheduleRepository.GetAll(cancellationToken);
                var dtos = all
                    .Select(RecurrencyScheduleMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();
                return Result.Success<IEnumerable<RecurrenceScheduleDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao encontrar agendamentos de recorrência com filtro");
                return Result.Failure<IEnumerable<RecurrenceScheduleDTO?>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RecurrenceScheduleDTO>>> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0) return Result.Failure<IEnumerable<RecurrenceScheduleDTO>>(RecurrenceScheduleErrors.InvalidId);

                var all = await _scheduleRepository.GetActiveByUserId(userId, cancellationToken);
                var dtos = all.Select(RecurrencyScheduleMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<RecurrenceScheduleDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter agendamentos de recorrência ativos para usuário {UserId}", userId);
                return Result.Failure<IEnumerable<RecurrenceScheduleDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RecurrenceScheduleDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _scheduleRepository.GetAll(cancellationToken);
                var dtos = all.Select(RecurrencyScheduleMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<RecurrenceScheduleDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os agendamentos de recorrência");
                return Result.Failure<IEnumerable<RecurrenceScheduleDTO?>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>> GetByFilterAsync(RecurrenceScheduleFilterDTO filter, CancellationToken cancellationToken)
        {
            try
            {
                var domainFilter = new RecurrenceScheduleQueryFilter(
                    filter.Id,
                    filter.TransactionId,
                    filter.UserId,
                    filter.Frequency,
                    filter.IsActive,
                    filter.StartDateFrom,
                    filter.StartDateTo,
                    filter.CreatedAt.HasValue ? DateOnly.FromDateTime(filter.CreatedAt.Value) : null,
                    filter.UpdatedAt.HasValue ? DateOnly.FromDateTime(filter.UpdatedAt.Value) : null,
                    filter.IsDeleted,
                    filter.SortBy,
                    filter.SortDesc,
                    filter.Page,
                    filter.PageSize);

                var (entities, totalItems) = await _scheduleRepository.FindByFilter(domainFilter, cancellationToken);
                if (!entities.Any())
                    return Result.Success<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>((new List<RecurrenceScheduleDTO>(), new PaginationData(filter.Page, filter.PageSize)));

                var dtos = entities
                    .Where(e => e != null)
                    .Select(RecurrencyScheduleMapper.ToDTO)
                    .ToList();

                var pageSize = filter.PageSize ?? 50;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var pagination = new PaginationData(filter.Page, pageSize, totalItems, totalPages);

                return Result.Success<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>((dtos, pagination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao converter filtro de agendamento de recorrência");
                return Result.Failure<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<RecurrenceScheduleDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0) return Result.Failure<RecurrenceScheduleDTO>(RecurrenceScheduleErrors.InvalidId);

                var entity = await _scheduleRepository.GetById(id, cancellationToken);
                if (entity is null) return Result.Failure<RecurrenceScheduleDTO>(RecurrenceScheduleErrors.NotFound(id));

                var dto = RecurrencyScheduleMapper.ToDTO(entity);

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter agendamento de recorrência com ID {ScheduleId}", id);
                return Result.Failure<RecurrenceScheduleDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<RecurrenceScheduleDTO>> GetByTransactionIdAsync(int transactionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _scheduleRepository.GetByTransactionId(transactionId, cancellationToken);
                if (entity is null) return Result.Failure<RecurrenceScheduleDTO>(Error.NullValue);

                var dto = RecurrencyScheduleMapper.ToDTO(entity);

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter agendamento de recorrência para transação {TransactionId}", transactionId);
                return Result.Failure<RecurrenceScheduleDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<RecurrenceScheduleDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _scheduleRepository.GetAll(cancellationToken);
                var totalCount = all.Count();
                var items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(RecurrencyScheduleMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<RecurrenceScheduleDTO?> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter agendamentos de recorrência paginados");
                return Result.Failure<(IEnumerable<RecurrenceScheduleDTO?> Items, int TotalCount)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> ProcessDueSchedulesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var now = DateTime.UtcNow;
                var dueSchedules = await _scheduleRepository.GetDueSchedules(now, cancellationToken);
                var transactionsCreated = 0;

                foreach (var schedule in dueSchedules)
                {
                    try
                    {
                        await using var dbTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                        var scheduleBatchCount = 0;

                        while (schedule.CanGenerateNext() && schedule.NextOccurrence <= now)
                        {
                            var nextDate = schedule.NextOccurrence;

                            var amount = schedule.Transaction.Amount;
                            var alreadyExists = await _dbContext.Transactions.AnyAsync(
                                t => t.UserId == schedule.Transaction.UserId
                                     && t.Description == schedule.Transaction.Description
                                     && t.Amount.Amount == amount.Amount
                                     && t.Amount.Currency == amount.Currency
                                     && t.TransactionDate == nextDate
                                     && !t.IsDeleted,
                                cancellationToken);

                            if (alreadyExists)
                            {
                                _logger.LogWarning(
                                    "Transação duplicada detectada para schedule {ScheduleId} na data {Date}. Avançando NextOccurrence.",
                                    schedule.Id, nextDate);

                                schedule.SkipOccurrence();
                                continue;
                            }

                            var transaction = schedule.GenerateTransaction();
                            await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
                            scheduleBatchCount++;
                        }

                        _dbContext.RecurrenceSchedule.Update(schedule);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        await dbTransaction.CommitAsync(cancellationToken);

                        transactionsCreated += scheduleBatchCount;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar schedule {ScheduleId}. Continuando com os demais.", schedule.Id);
                    }
                }

                _logger.LogInformation("Processamento concluído. {Count} transações criadas", transactionsCreated);

                return Result.Success(transactionsCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar agendamentos de recorrência");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateRecurrenceScheduleDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto is null) return Result.Failure<bool>(Error.NullValue);
                if (dto.Id <= 0) return Result.Failure<bool>(RecurrenceScheduleErrors.InvalidId);

                var entity = await _scheduleRepository.GetById(dto.Id, cancellationToken);
                if (entity == null) return Result.Failure<bool>(RecurrenceScheduleErrors.NotFound(dto.Id));

                entity.Update(dto.Frequency, dto.EndDate, dto.MaxOccurrences);
                await _scheduleRepository.Update(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar agendamento de recorrência com ID {ScheduleId}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}
