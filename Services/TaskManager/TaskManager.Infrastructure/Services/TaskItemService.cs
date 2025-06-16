using System.Linq.Expressions;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Infrastructure.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public TaskItemService(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<TaskItemDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            TaskItem? taskItem = await _taskItemRepository.GetById(id, cancellationToken);
            if (taskItem == null) return null;

            TaskItemDTO taskItemDTIO = taskItem.ToDTO();
            return taskItemDTIO;
        }

        public async Task<IEnumerable<TaskItemDTO>> GetByFilterAsync(TaskItemFilterDTO filter, CancellationToken cancellationToken)
        {
            TaskItemFilter? domainFilter = new(
                filter.UserId,
                filter.TitleContains,
                filter.Status,
                filter.Priority,
                filter.DueDate,
                filter.LabelId);

            IEnumerable<TaskItem?> entities = await _taskItemRepository.FindByFilter(domainFilter, cancellationToken);
            IEnumerable<TaskItemDTO> dtos = entities.Select(TaskItemMapper.ToDTO);
            return dtos;
        }

        public async Task<IEnumerable<TaskItemDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> entities = await _taskItemRepository.GetAll(cancellationToken);
            IEnumerable<TaskItemDTO> dtos = entities.Select(TaskItemMapper.ToDTO);
            return dtos;
        }

        public async Task<int> CreateAsync(CreateTaskItemDTO dto, CancellationToken cancellationToken)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            TaskItem entity = new(dto.Title, dto.Description, dto.Priority, dto.DueDate, dto.UserId);
            await _taskItemRepository.Create(entity, cancellationToken);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, string title, string description, int status, int priority, DateOnly dueDate, CancellationToken cancellationToken)
        {
            TaskItem? entity = await _taskItemRepository.GetById(id, cancellationToken);
            if (entity == null) return false;

            entity.Update(title, description, (Status)status, (Priority)priority, dueDate);
            await _taskItemRepository.Update(entity, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            TaskItem? entity = await _taskItemRepository.GetById(id, cancellationToken);
            if (entity == null) return false;

            entity.MarkAsDeleted();
            await _taskItemRepository.Update(entity, cancellationToken);
            return true;
        }

        public async Task<(IEnumerable<TaskItemDTO> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem?> all = await _taskItemRepository.GetAll(cancellationToken);
            int totalCount = all.Count();

            IEnumerable<TaskItemDTO?> items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(TaskItemMapper.ToDTO!);

            return (items, totalCount);
        }

        public async Task<IEnumerable<TaskItemDTO>> FindAsync(Expression<Func<TaskItemDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem?> entities = await _taskItemRepository.GetAll(cancellationToken);
            IEnumerable<TaskItemDTO?> dtos = entities.Select(TaskItemMapper.ToDTO!);
            return dtos;
        }

        public async Task<int> CountAsync(Expression<Func<TaskItemDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _taskItemRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<IEnumerable<int>> CreateRangeAsync(IEnumerable<CreateTaskItemDTO> dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            IEnumerable<TaskItem> entities = dto.Select(TaskItemMapper.ToEntity);
            await _taskItemRepository.CreateRange(entities, cancellationToken);
            return entities.Select(e => e.Id).ToList();
        }

        public async Task<bool> UpdateAsync(UpdateTaskItemDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            TaskItem? entity = await _taskItemRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            entity.Update(dto.Title, dto.Description, dto.Status, dto.Priority, dto.DueDate);
            entity.MarkAsUpdated();

            await _taskItemRepository.Update(entity, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> dtos, CancellationToken cancellationToken = default)
        {
            if (dtos == null || !dtos.Any()) return false;

            List<TaskItem?> entities = new();
            foreach (var id in dtos)
            {
                TaskItem? entity = await _taskItemRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }

            if (!entities.Any()) return false;

            foreach (var entity in entities)
                await _taskItemRepository.Delete(entity, cancellationToken);

            return true;
        }
    }
}
