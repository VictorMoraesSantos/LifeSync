using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Errors;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.Filters;

namespace TaskManager.Infrastructure.Services
{
    public class TaskLabelService : ITaskLabelService
    {
        private readonly ITaskLabelRepository _taskLabelRepository;
        private readonly ILogger<TaskLabelService> _logger;

        public TaskLabelService(
            ITaskLabelRepository taskLabelRepository,
            ILogger<TaskLabelService> logger)
        {
            _taskLabelRepository = taskLabelRepository ?? throw new ArgumentNullException(nameof(taskLabelRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<IEnumerable<TaskLabelDTO>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var entities = await _taskLabelRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<TaskLabelDTO>>(new List<TaskLabelDTO>());

                var dtos = entities.Where(e => e != null).Select(tl => tl.ToDTO()).ToList();

                return Result.Success<IEnumerable<TaskLabelDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os rótulos");
                return Result.Failure<IEnumerable<TaskLabelDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TaskLabelDTO>>> GetByFilterAsync(TaskLabelFilterDTO filter, CancellationToken cancellationToken)
        {
            try
            {
                var domainFilter = new TaskLabelFilter(
                    filter.UserId,
                    filter.TaskItemId,
                    filter.NameContains,
                    filter.LabelColor);

                var entities = await _taskLabelRepository.FindByFilter(domainFilter, cancellationToken);
                var dtos = entities.Where(e => e != null).Select(TaskLabelMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<TaskLabelDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao filtrar rótulos");
                return Result.Failure<IEnumerable<TaskLabelDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<TaskLabelDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _taskLabelRepository.GetById(id, cancellationToken);

                if (entity == null)
                    return Result.Failure<TaskLabelDTO>(TaskLabelErrors.NotFound(id));

                var dto = entity.ToDTO();
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar rótulo {LabelId}", id);
                return Result.Failure<TaskLabelDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateTaskLabelDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = dto.ToEntity();
                await _taskLabelRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Rótulo criado com sucesso: {LabelId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar rótulo {@LabelData}", dto);
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateTaskLabelDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _taskLabelRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(TaskLabelErrors.NotFound(dto.Id));

                entity.Update(dto.Name, dto.LabelColor);
                await _taskLabelRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Rótulo atualizado com sucesso: {LabelId}", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar rótulo {@LabelData}", dto);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _taskLabelRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(TaskLabelErrors.NotFound(id));

                await _taskLabelRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Rótulo excluído com sucesso: {LabelId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir rótulo {LabelId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<TaskLabelDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<TaskLabelDTO>, int)>(TaskLabelErrors.InvalidPagination);

                var all = await _taskLabelRepository.GetAll(cancellationToken);
                var totalCount = all.Count();

                var items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Where(e => e != null)
                    .Select(TaskLabelMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<TaskLabelDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de rótulos (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<TaskLabelDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TaskLabelDTO>>> FindAsync(Expression<Func<TaskLabelDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<TaskLabelDTO>>(Error.NullValue);

                var entities = await _taskLabelRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(TaskLabelMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<TaskLabelDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar rótulos com predicado");
                return Result.Failure<IEnumerable<TaskLabelDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<TaskLabelDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _taskLabelRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(TaskLabelMapper.ToDTO)
                    .AsQueryable();

                int count = predicate != null ? dtos.Count(predicate) : dtos.Count();
                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar rótulos");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateTaskLabelDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(TaskLabelErrors.EmptyOrNullList);

                var entities = new List<TaskLabel>();
                var errors = new List<(string Name, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = dto.ToEntity();
                        entities.Add(entity);
                    }
                    catch (DomainException ex)
                    {
                        errors.Add((dto.Name ?? "Sem nome", ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Name}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Alguns rótulos possuem dados inválidos: {errorDetails}"));
                }

                await _taskLabelRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criados {Count} rótulos com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos rótulos");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(TaskLabelErrors.InvalidIds);

                var entities = new List<TaskLabel>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _taskLabelRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                    return Result.Failure<bool>(TaskLabelErrors.SomeNotFound(notFoundIds));

                if (!entities.Any())
                    return Result.Failure<bool>(TaskLabelErrors.AllNotFound);

                foreach (var entity in entities)
                {
                    entity.MarkAsDeleted();
                    await _taskLabelRepository.Update(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídos {Count} rótulos com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos rótulos {LabelIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}