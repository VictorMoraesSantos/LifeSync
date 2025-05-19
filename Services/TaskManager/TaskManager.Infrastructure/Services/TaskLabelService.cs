using BuildingBlocks.Exceptions;
using System.Linq.Expressions;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.ValueObjects;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure.Services
{
    public class TaskLabelService : ITaskLabelService
    {
        private readonly ITaskLabelRepository _taskLabelRepository;

        public TaskLabelService(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }

        public async Task<IEnumerable<TaskLabelDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabel?> entities = await _taskLabelRepository.GetAll(cancellationToken);
            IEnumerable<TaskLabelDTO> dtos = entities.Select(tl => tl.ToDTO());
            return dtos;
        }

        public async Task<IEnumerable<TaskLabelDTO>> GetByFilterAsync(TaskLabelFilterDTO filter, CancellationToken cancellationToken)
        {
            TaskLabelFilter? domainFilter = new(
                filter.UserId,
                filter.TaskItemId,
                filter.NameContains,
                filter.LabelColor);

            IEnumerable<TaskLabel?> entities = await _taskLabelRepository.FindByFilter(domainFilter, cancellationToken);
            IEnumerable<TaskLabelDTO> dtos = entities.Select(TaskLabelMapper.ToDTO);
            return dtos;
        }

        public async Task<TaskLabelDTO?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            TaskLabel? entities = await _taskLabelRepository.GetById(id, cancellationToken);
            if (entities == null)
                return null;

            TaskLabelDTO dtos = TaskLabelMapper.ToDTO(entities);
            return dtos;
        }

        public async Task<bool> CreateAsync(CreateTaskLabelDTO dto, CancellationToken cancellationToken)
        {
            TaskLabel entity = new(dto.Name, dto.LabelColor, dto.UserId, dto.TaskItemId);
            await _taskLabelRepository.Create(entity, cancellationToken);
            return true;
        }

        public async Task<bool> UpdateAsync(UpdateTaskLabelDTO dto, CancellationToken cancellationToken)
        {
            TaskLabel? entity = await _taskLabelRepository.GetById(dto.Id, cancellationToken);
            if (entity == null)
                throw new NotFoundException(nameof(entity), dto.Id);

            entity.Update(dto.Name, dto.LabelColor);
            await _taskLabelRepository.Update(entity, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            TaskLabel? entity = await _taskLabelRepository.GetById(id, cancellationToken);
            if (entity == null) return false;

            entity.MarkAsDeleted();
            await _taskLabelRepository.Update(entity, cancellationToken);
            return true;
        }

        public async Task<(IEnumerable<TaskLabelDTO?> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel?> all = await _taskLabelRepository.GetAll(cancellationToken);
            int totalCount = all.Count();

            IEnumerable<TaskLabelDTO?> items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(TaskLabelMapper.ToDTO!);

            return (items, totalCount);
        }

        public async Task<IEnumerable<TaskLabelDTO?>> FindAsync(Expression<Func<TaskLabelDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel?> entities = await _taskLabelRepository.GetAll(cancellationToken);
            IEnumerable<TaskLabelDTO?> dtos = entities.Select(TaskLabelMapper.ToDTO!);
            return dtos;
        }

        public async Task<int> CountAsync(Expression<Func<TaskLabelDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _taskLabelRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<bool> CreateRangeAsync(IEnumerable<CreateTaskLabelDTO> dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            IEnumerable<TaskLabel> entities = dto.Select(TaskLabelMapper.ToEntity);

            await _taskLabelRepository.CreateRange(entities, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> dtos, CancellationToken cancellationToken = default)
        {
            if (dtos == null || !dtos.Any()) return false;

            List<TaskLabel?> entities = new();
            foreach (var id in dtos)
            {
                TaskLabel? entity = await _taskLabelRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }

            if (!entities.Any()) return false;

            foreach (var entity in entities)
                await _taskLabelRepository.Delete(entity, cancellationToken);

            return true;
        }
    }
}
