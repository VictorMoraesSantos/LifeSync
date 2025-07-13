using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Errors;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class DailyProgressService : IDailyProgressService
    {
        private readonly IDailyProgressRepository _dailyProgressRepository;
        private readonly ILogger<DailyProgressService> _logger;

        public DailyProgressService(
            IDailyProgressRepository dailyProgressRepository,
            ILogger<DailyProgressService> logger)
        {
            _dailyProgressRepository = dailyProgressRepository ?? throw new ArgumentNullException(nameof(dailyProgressRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<DailyProgressDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var dailyProgress = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (dailyProgress == null)
                    return Result.Failure<DailyProgressDTO>(DailyProgressErrors.NotFound(id));

                var dailyProgressDTO = DailyProgressMapper.ToDTO(dailyProgress);

                return Result.Success(dailyProgressDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar progresso diário {ProgressId}", id);
                return Result.Failure<DailyProgressDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<DailyProgressDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<DailyProgressDTO>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _dailyProgressRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Where(e => e != null)
                    .Select(DailyProgressMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<DailyProgressDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de progresso diário (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<DailyProgressDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<DailyProgressDTO>>> FindAsync(Expression<Func<DailyProgressDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.NullValue);

                var entities = await _dailyProgressRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(DailyProgressMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<DailyProgressDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar progresso diário com predicado");
                return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<DailyProgressDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _dailyProgressRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(DailyProgressMapper.ToDTO)
                    .AsQueryable();
                int count = predicate != null ? dtos.Count(predicate) : dtos.Count();

                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar registros de progresso diário");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<DailyProgressDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _dailyProgressRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<DailyProgressDTO>>(new List<DailyProgressDTO>());

                var dtos = entities.Where(e => e != null).Select(DailyProgressMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<DailyProgressDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os registros de progresso diário");
                return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<DailyProgressDTO>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                    return Result.Failure<IEnumerable<DailyProgressDTO>>(DailyProgressErrors.InvalidUserId);

                var entities = await _dailyProgressRepository.GetAllByUserId(userId, cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<DailyProgressDTO>>(new List<DailyProgressDTO>());

                var dtos = entities.Where(e => e != null).Select(DailyProgressMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<DailyProgressDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar registros de progresso diário do usuário {UserId}", userId);
                return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateDailyProgressDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = DailyProgressMapper.ToEntity(dto);
                await _dailyProgressRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Progresso diário criado com sucesso: {ProgressId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar progresso diário {@ProgressData}", dto);
                return Result.Failure<int>(DailyProgressErrors.CreateError);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateDailyProgressDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("Lista de progresso diário inválida ou vazia"));

                var entities = new List<DailyProgress>();
                var errors = new List<(DateOnly Date, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = DailyProgressMapper.ToEntity(dto);
                        entities.Add(entity);
                    }
                    catch (DomainException ex)
                    {
                        errors.Add((dto.Date, ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Date:yyyy-MM-dd}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Alguns registros de progresso possuem dados inválidos: {errorDetails}"));
                }

                await _dailyProgressRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criados {Count} registros de progresso diário com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos registros de progresso diário");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateDailyProgressDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _dailyProgressRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(DailyProgressErrors.NotFound(dto.Id));

                entity.SetConsumed(dto.CaloriesConsumed, dto.LiquidsConsumedMl);

                entity.MarkAsUpdated();

                await _dailyProgressRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Progresso diário atualizado com sucesso: {ProgressId}", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar progresso diário {ProgressId}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> SetConsumedAsync(int id, int calories, int liquidsMl, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(DailyProgressErrors.NotFound(id));

                entity.SetConsumed(calories, liquidsMl);

                entity.MarkAsUpdated();

                await _dailyProgressRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Consumo definido com sucesso para progresso diário: {ProgressId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao definir consumo para progresso diário {ProgressId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> SetGoalAsync(int id, DailyGoalDTO goalDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (goalDto == null)
                    return Result.Failure<bool>(DailyProgressErrors.NullGoal);

                var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(DailyProgressErrors.NotFound(id));

                var goal = DailyGoalMapper.ToEntity(goalDto);

                entity.SetGoal(goal);

                entity.MarkAsUpdated();

                await _dailyProgressRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Meta definida com sucesso para progresso diário: {ProgressId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao definir meta para progresso diário {ProgressId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(DailyProgressErrors.NotFound(id));

                await _dailyProgressRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Progresso diário excluído com sucesso: {ProgressId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir progresso diário {ProgressId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<DailyProgress>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"Os seguintes registros de progresso não foram encontrados: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Nenhum dos registros de progresso foi encontrado"));

                foreach (var entity in entities)
                {
                    await _dailyProgressRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídos {Count} registros de progresso diário com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos registros de progresso diário {ProgressIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}