using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Errors;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.ValueObjects;

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
            _taskItemRepository = taskItemRepository ?? throw new ArgumentNullException(nameof(taskItemRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<TaskItemDTO>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var taskItem = await _taskItemRepository.GetById(id, cancellationToken);

                if (taskItem == null)
                    return Result.Failure<TaskItemDTO>(TaskItemErrors.NotFound(id).Description);

                var taskItemDTO = taskItem.ToDTO();
                return Result.Success(taskItemDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefa {TaskId}", id);
                return Result.Failure<TaskItemDTO>(Error.Problem(
                    "TaskItem.GetByIdError",
                    "Erro ao buscar tarefa").Description);
            }
        }

        public async Task<Result<IEnumerable<TaskItemDTO>>> GetByFilterAsync(TaskItemFilterDTO filter, CancellationToken cancellationToken)
        {
            try
            {
                var domainFilter = new TaskItemFilter(
                    filter.UserId,
                    filter.TitleContains,
                    filter.Status,
                    filter.Priority,
                    filter.DueDate,
                    filter.LabelId);

                var entities = await _taskItemRepository.FindByFilter(domainFilter, cancellationToken);
                var dtos = entities.Where(e => e != null).Select(TaskItemMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<TaskItemDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao filtrar tarefas {@Filter}", filter);
                return Result.Failure<IEnumerable<TaskItemDTO>>(Error.Problem(
                    "TaskItem.FilterError",
                    "Erro ao filtrar tarefas").Description);
            }
        }

        public async Task<Result<IEnumerable<TaskItemDTO>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var entities = await _taskItemRepository.GetAll(cancellationToken);
                var dtos = entities.Where(e => e != null).Select(TaskItemMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<TaskItemDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as tarefas");
                return Result.Failure<IEnumerable<TaskItemDTO>>(Error.Problem(
                    "TaskItem.GetAllError",
                    "Erro ao buscar todas as tarefas").Description);
            }
        }

        public async Task<Result<int>> CreateAsync(CreateTaskItemDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.Failure(
                        "TaskItem.NullData",
                        "Dados da tarefa não podem ser nulos").Description);

                var entity = new TaskItem(dto.Title, dto.Description, dto.Priority, dto.DueDate, dto.UserId);
                await _taskItemRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Tarefa criada com sucesso: {TaskId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Core.Domain.Exceptions.DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao criar tarefa {@TaskData}", dto);
                return Result.Failure<int>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa {@TaskData}", dto);
                return Result.Failure<int>(TaskItemErrors.CreateError.Description);
            }
        }

        public async Task<Result<bool>> UpdateAsync(int id, string title, string description, int status, int priority, DateOnly dueDate, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _taskItemRepository.GetById(id, cancellationToken);

                if (entity == null)
                    return Result.Failure<bool>(TaskItemErrors.NotFound(id).Description);

                entity.Update(title, description, (Status)status, (Priority)priority, dueDate);
                await _taskItemRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Tarefa atualizada com sucesso: {TaskId}", id);
                return Result.Success(true);
            }
            catch (Core.Domain.Exceptions.DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao atualizar tarefa {TaskId}", id);
                return Result.Failure<bool>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa {TaskId}", id);
                return Result.Failure<bool>(TaskItemErrors.UpdateError.Description);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _taskItemRepository.GetById(id, cancellationToken);

                if (entity == null)
                    return Result.Failure<bool>(TaskItemErrors.NotFound(id).Description);

                entity.MarkAsDeleted();
                await _taskItemRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Tarefa marcada como excluída: {TaskId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir tarefa {TaskId}", id);
                return Result.Failure<bool>(TaskItemErrors.DeleteError.Description);
            }
        }

        public async Task<Result<(IEnumerable<TaskItemDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1)
                    return Result.Failure<(IEnumerable<TaskItemDTO>, int)>(Error.Failure(
                        "TaskItem.InvalidPage",
                        "O número da página deve ser maior que zero").Description);

                if (pageSize < 1)
                    return Result.Failure<(IEnumerable<TaskItemDTO>, int)>(Error.Failure(
                        "TaskItem.InvalidPageSize",
                        "O tamanho da página deve ser maior que zero").Description);

                var all = await _taskItemRepository.GetAll(cancellationToken);
                var totalCount = all.Count();

                var items = all
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
                return Result.Failure<(IEnumerable<TaskItemDTO>, int)>(Error.Problem(
                    "TaskItem.GetPagedError",
                    "Erro ao obter página de tarefas").Description);
            }
        }

        public async Task<Result<IEnumerable<TaskItemDTO>>> FindAsync(Expression<Func<TaskItemDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<TaskItemDTO>>(Error.Failure(
                        "TaskItem.NullPredicate",
                        "O predicado de busca não pode ser nulo").Description);

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
                return Result.Failure<IEnumerable<TaskItemDTO>>(Error.Problem(
                    "TaskItem.FindError",
                    "Erro ao buscar tarefas").Description);
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
                return Result.Failure<int>(Error.Problem(
                    "TaskItem.CountError",
                    "Erro ao contar tarefas").Description);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateTaskItemDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null)
                    return Result.Failure<IEnumerable<int>>(Error.Failure(
                        "TaskItem.NullData",
                        "Lista de tarefas não pode ser nula").Description);

                if (!dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure(
                        "TaskItem.EmptyList",
                        "Lista de tarefas está vazia").Description);

                // Validação detalhada de cada item
                var invalidItems = new List<(string Item, string Error)>();
                foreach (var dto in dtos)
                {
                    if (string.IsNullOrWhiteSpace(dto.Title))
                        invalidItems.Add((dto.Title ?? "Sem título", TaskItemErrors.InvalidTitle.Description));
                    else if (dto.DueDate < DateOnly.FromDateTime(DateTime.Today))
                        invalidItems.Add((dto.Title, TaskItemErrors.DueDateInPast.Description));
                }

                if (invalidItems.Any())
                {
                    var errorDetails = string.Join("; ", invalidItems.Select(i => $"'{i.Item}': {i.Error}"));
                    return Result.Failure<IEnumerable<int>>(
                        $"Há tarefas com dados inválidos na lista. Detalhes: {errorDetails}");
                }

                var entities = dtos.Select(dto => new TaskItem(dto.Title, dto.Description, dto.Priority, dto.DueDate, dto.UserId)).ToList();
                await _taskItemRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criadas {Count} tarefas com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Core.Domain.Exceptions.DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao criar múltiplas tarefas");
                return Result.Failure<IEnumerable<int>>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplas tarefas");
                return Result.Failure<IEnumerable<int>>(Error.Problem(
                    "TaskItem.CreateRangeError",
                    "Erro ao criar múltiplas tarefas").Description);
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateTaskItemDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.Failure(
                        "TaskItem.NullData",
                        "Dados da tarefa não podem ser nulos").Description);

                var entity = await _taskItemRepository.GetById(dto.Id, cancellationToken);

                if (entity == null)
                    return Result.Failure<bool>(TaskItemErrors.NotFound(dto.Id).Description);

                entity.Update(dto.Title, dto.Description, dto.Status, dto.Priority, dto.DueDate);
                await _taskItemRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Tarefa atualizada com sucesso: {TaskId}", dto.Id);
                return Result.Success(true);
            }
            catch (Core.Domain.Exceptions.DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao atualizar tarefa {@TaskData}", dto);
                return Result.Failure<bool>(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa {@TaskData}", dto);
                return Result.Failure<bool>(TaskItemErrors.UpdateError.Description);
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure(
                        "TaskItem.InvalidIds",
                        "Lista de IDs inválida ou vazia").Description);

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
                    return Result.Failure<bool>(Error.NotFound(
                        "TaskItem.SomeNotFound",
                        $"As seguintes tarefas não foram encontradas: {idsText}").Description);
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound(
                        "TaskItem.AllNotFound",
                        "Nenhuma das tarefas foi encontrada").Description);

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
                return Result.Failure<bool>(Error.Problem(
                    "TaskItem.DeleteRangeError",
                    "Erro ao excluir múltiplas tarefas").Description);
            }
        }
    }
}