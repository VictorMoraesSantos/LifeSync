using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Core.Domain.Notifications;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Errors;
using TaskManager.Domain.Filters;
using TaskManager.Domain.Repositories;

namespace TaskManager.Infrastructure.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly ILogger<TaskItemService> _logger;

        public TaskItemService(
            ITaskItemRepository taskItemRepository,
            ILogger<TaskItemService> logger)
        {
            _taskItemRepository = taskItemRepository;
            _logger = logger;
        }

        public async Task<Result<TaskItemDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var taskItem = await _taskItemRepository.GetById(id, cancellationToken);
                if (taskItem == null)
                    return Result.Failure<TaskItemDTO>(TaskItemErrors.NotFound(id));

                var taskItemDTO = taskItem.ToDTO();

                return Result.Success(taskItemDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefa {TaskId}", id);
                return Result.Failure<TaskItemDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<TaskItemDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<TaskItemDTO>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _taskItemRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Where(e => e != null)
                    .Select(TaskItemMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<TaskItemDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de tarefas (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<TaskItemDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<TaskItemDTO> Items, PaginationData Pagination)>> GetByFilterAsync(TaskItemFilterDTO filter, CancellationToken cancellationToken)
        {
            try
            {

                var domainFilter = new TaskItemQueryFilter(
                    filter.Id,
                    filter.UserId,
                    filter.TitleContains,
                    filter.Status,
                    filter.Priority,
                    filter.DueDate,
                    filter.LabelId,
                    filter.CreatedAt,
                    filter.UpdatedAt,
                    filter.IsDeleted,
                    filter.SortBy,
                    filter.SortDesc,
                    filter.Page,
                    filter.PageSize);

                var (entities, totalItems) = await _taskItemRepository.FindByFilter(domainFilter, cancellationToken);
                if (!entities.Any())
                    return Result.Success<(IEnumerable<TaskItemDTO> Items, PaginationData Pagination)>((new List<TaskItemDTO>(), new PaginationData(filter.Page, filter.PageSize)));

                var dtos = entities
                    .Where(e => e != null)
                    .Select(TaskItemMapper.ToDTO)
                    .ToList();

                var pageSize = filter.PageSize ?? 50;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var pagination = new PaginationData(filter.Page, pageSize, totalItems, totalPages);

                return Result.Success<(IEnumerable<TaskItemDTO> Items, PaginationData Pagination)>((dtos, pagination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas com filtro {@Filter}", filter);
                return Result.Failure<(IEnumerable<TaskItemDTO>, PaginationData)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TaskItemDTO>>> FindAsync(Expression<Func<TaskItemDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<TaskItemDTO>>(Error.NullValue);

                var entities = await _taskItemRepository.GetAll(cancellationToken);

                var dtos = entities
                    .Where(e => e != null)
                    .Select(TaskItemMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<TaskItemDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas com predicado");
                return Result.Failure<IEnumerable<TaskItemDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<TaskItemDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _taskItemRepository.GetAll(cancellationToken);

                var dtos = all
                    .Where(e => e != null)
                    .Select(TaskItemMapper.ToDTO)
                    .AsQueryable();

                int count = predicate != null ? dtos.Count(predicate) : dtos.Count();

                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar tarefas");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<TaskItemDTO>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var entities = await _taskItemRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<TaskItemDTO>>(new List<TaskItemDTO>());

                var dtos = entities.Select(TaskItemMapper.ToDTO!).ToList();

                return Result.Success<IEnumerable<TaskItemDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as tarefas");
                return Result.Failure<IEnumerable<TaskItemDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateTaskItemDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = dto.ToEntity();

                if (entity.IsInvalid)
                {
                    var errorMessages = entity.GetErrorMessagesAsString();
                    var errorString = string.Join("; ", errorMessages);

                    _logger.LogWarning("Falha na validação de domínio ao criar tarefa: {Errors}", errorString);

                    return Result.Failure<int>(new Error(errorString, ErrorType.Validation));
                }

                await _taskItemRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Tarefa criada com sucesso: {TaskId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa {@TaskData}", dto);
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateTaskItemDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure(Error.NullValue.Description));

                var entities = new List<TaskItem>();
                var errors = new List<(string Title, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = dto.ToEntity();
                        entities.Add(entity);
                    }
                    catch (DomainException ex)
                    {
                        errors.Add((dto.Title ?? "Sem título", ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Title}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Algumas tarefas possuem dados inválidos: {errorDetails}"));
                }

                await _taskItemRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criadas {Count} tarefas com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplas tarefas");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateTaskItemDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _taskItemRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(TaskItemErrors.NotFound(dto.Id));

                entity.Update(dto.Title, dto.Description, dto.Status, dto.Priority, dto.DueDate);

                await _taskItemRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Tarefa atualizada com sucesso: {TaskId}", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa {TaskId}", dto.Id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _taskItemRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(TaskItemErrors.NotFound(id));

                await _taskItemRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Tarefa excluída com sucesso: {TaskId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir tarefa {TaskId}", id);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<TaskItem>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _taskItemRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"As seguintes tarefas não foram encontradas: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Nenhuma das tarefas foi encontrada"));

                foreach (var entity in entities)
                {
                    entity.MarkAsDeleted();
                    await _taskItemRepository.Update(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídas {Count} tarefas com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplas tarefas {TaskIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }
    }
}