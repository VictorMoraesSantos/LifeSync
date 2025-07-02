using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
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
            _dailyProgressRepository = dailyProgressRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<DailyProgressDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _dailyProgressRepository.GetAll(cancellationToken);
                return Result.Success(all.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar registros de progresso diário");
                return Result.Failure<int>(Error.Problem("DailyProgress", "CountError").Description);
            }
        }

        public async Task<Result<int>> CreateAsync(CreateDailyProgressDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.Failure("General", "NullData").Description);

                // Validações adicionais podem ser adicionadas aqui
                if (dto.Date > DateOnly.FromDateTime(DateTime.Today))
                    return Result.Failure<int>(Error.Failure("DailyProgress", "FutureDate").Description);

                var entity = DailyProgressMapper.ToEntity(dto);
                await _dailyProgressRepository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar progresso diário {@ProgressData}", dto);
                return Result.Failure<int>(Error.Problem("DailyProgress", "CreateError").Description);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateDailyProgressDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null)
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "NullData").Description);

                if (!dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "EmptyList").Description);

                // Validação em lote
                var invalidItems = dtos.Where(dto => dto.Date > DateOnly.FromDateTime(DateTime.Today)).ToList();
                if (invalidItems.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("DailyProgress", "FutureDatesInList").Description);

                var entities = dtos.Select(DailyProgressMapper.ToEntity);
                await _dailyProgressRepository.CreateRange(entities, cancellationToken);

                return Result.Success<IEnumerable<int>>(entities.Select(entity => entity.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos registros de progresso diário");
                return Result.Failure<IEnumerable<int>>(Error.Problem("DailyProgress", "CreateRangeError").Description);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("DailyProgress", "NotFound").Description);

                await _dailyProgressRepository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir progresso diário {ProgressId}", id);
                return Result.Failure<bool>(Error.Problem("DailyProgress", "DeleteError").Description);
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("General", "EmptyList").Description);

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
                    return Result.Failure<bool>(Error.NotFound("DailyProgress", "SomeNotFound").Description);

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("DailyProgress", "AllNotFound").Description);

                foreach (var entity in entities)
                    await _dailyProgressRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos registros de progresso diário {ProgressIds}", ids);
                return Result.Failure<bool>(Error.Problem("DailyProgress", "DeleteRangeError").Description);
            }
        }

        public async Task<Result<IEnumerable<DailyProgressDTO>>> FindAsync(Expression<Func<DailyProgressDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.Failure("General", "NullData").Description);

                // Para simplificar, não implementa filtro por predicate no momento
                var entities = await _dailyProgressRepository.GetAll(cancellationToken);
                var dtos = entities.Select(DailyProgressMapper.ToDTO);

                return Result.Success<IEnumerable<DailyProgressDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar registros de progresso diário com predicado");
                return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.Problem("DailyProgress", "FindError").Description);
            }
        }

        public async Task<Result<IEnumerable<DailyProgressDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _dailyProgressRepository.GetAll(cancellationToken);
                var dtos = entities.Select(DailyProgressMapper.ToDTO);

                return Result.Success<IEnumerable<DailyProgressDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os registros de progresso diário");
                return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.Problem("DailyProgress", "GetAllError").Description);
            }
        }

        public async Task<Result<DailyProgressDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<DailyProgressDTO>(Error.Failure("General", "InvalidId").Description);

                var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<DailyProgressDTO>(Error.NotFound("DailyProgress", "NotFound").Description);

                var dto = DailyProgressMapper.ToDTO(entity);
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar progresso diário {ProgressId}", id);
                return Result.Failure<DailyProgressDTO>(Error.Problem("DailyProgress", "GetByIdError").Description);
            }
        }

        public async Task<Result<IEnumerable<DailyProgressDTO>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId <= 0)
                    return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.Failure("General", "InvalidId").Description);

                var entities = await _dailyProgressRepository.GetAllByUserId(userId, cancellationToken);
                var dtos = entities.Select(DailyProgressMapper.ToDTO);

                return Result.Success<IEnumerable<DailyProgressDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar registros de progresso diário do usuário {UserId}", userId);
                return Result.Failure<IEnumerable<DailyProgressDTO>>(Error.Problem("DailyProgress", "GetByUserIdError").Description);
            }
        }

        public async Task<Result<(IEnumerable<DailyProgressDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1)
                    return Result.Failure<(IEnumerable<DailyProgressDTO>, int)>(Error.Failure("DailyProgress", "InvalidPage").Description);

                if (pageSize < 1)
                    return Result.Failure<(IEnumerable<DailyProgressDTO>, int)>(Error.Failure("DailyProgress", "InvalidPageSize").Description);

                var all = await _dailyProgressRepository.GetAll(cancellationToken);
                int totalCount = all.Count();

                var items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(DailyProgressMapper.ToDTO);

                return Result.Success<(IEnumerable<DailyProgressDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de registros de progresso diário (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<DailyProgressDTO>, int)>(Error.Problem("DailyProgress", "GetPagedError").Description);
            }
        }

        public async Task<Result<bool>> SetConsumedAsync(int id, int calories, int liquidsMl, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                if (calories < 0)
                    return Result.Failure<bool>(Error.Failure("DailyProgress", "NegativeCalories").Description);

                if (liquidsMl < 0)
                    return Result.Failure<bool>(Error.Failure("DailyProgress", "NegativeLiquids").Description);

                var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("DailyProgress", "NotFound").Description);

                entity.SetConsumed(calories, liquidsMl);
                await _dailyProgressRepository.Update(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao definir consumo para progresso diário {ProgressId}", id);
                return Result.Failure<bool>(Error.Problem("DailyProgress", "SetConsumedError").Description);
            }
        }

        public async Task<Result<bool>> SetGoalAsync(int id, DailyGoalDTO goalDto, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                if (goalDto == null)
                    return Result.Failure<bool>(Error.Failure("General", "NullData").Description);

                var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("DailyProgress", "NotFound").Description);

                var goal = DailyGoalMapper.ToEntity(goalDto);
                entity.SetGoal(goal);
                await _dailyProgressRepository.Update(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao definir meta para progresso diário {ProgressId}", id);
                return Result.Failure<bool>(Error.Problem("DailyProgress", "SetGoalError").Description);
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateDailyProgressDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.Failure("General", "NullData").Description);

                if (dto.Id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                if (dto.CaloriesConsumed < 0)
                    return Result.Failure<bool>(Error.Failure("DailyProgress", "NegativeCalories").Description);

                if (dto.LiquidsConsumedMl < 0)
                    return Result.Failure<bool>(Error.Failure("DailyProgress", "NegativeLiquids").Description);

                var entity = await _dailyProgressRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("DailyProgress", "NotFound").Description);

                // Atualize os campos necessários
                entity.SetConsumed(dto.CaloriesConsumed, dto.LiquidsConsumedMl);

                await _dailyProgressRepository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar progresso diário {@ProgressData}", dto);
                return Result.Failure<bool>(Error.Problem("DailyProgress", "UpdateError").Description);
            }
        }
    }
}