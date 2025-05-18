using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TaskManager.Infrastructure.Services
{
    public class TaskLabelService : ITaskLabelService
    {
        private readonly ITaskLabelRepository _taskLabelRepository;

        public TaskLabelService(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }

        public async Task<IEnumerable<TaskLabelDTO>> GetAllTaskLabelsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabel?> taskLabels = await _taskLabelRepository.GetAll(cancellationToken);
            IEnumerable<TaskLabelDTO> taskLabelDTOs = taskLabels.Select(tl => tl.ToDTO());
            return taskLabelDTOs;
        }

        public async Task<TaskLabelDTO> GetTaskLabelByIdAsync(int id, CancellationToken cancellationToken)
        {
            TaskLabel? taskLabel = await _taskLabelRepository.GetById(id, cancellationToken);
            if (taskLabel == null)
                throw new NotFoundException(nameof(TaskLabel), id);

            TaskLabelDTO taskLabelDTO = TaskLabelMapper.ToDTO(taskLabel);
            return taskLabelDTO;
        }

        public async Task<IEnumerable<TaskLabelDTO>> GetTaskLabelsByNameAsync(int userId, string name, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabel?> taskLabels = await _taskLabelRepository.GetByName(userId, name, cancellationToken);
            if (taskLabels == null || !taskLabels.Any())
                throw new NotFoundException("TaskLabel cannot be found");

            IEnumerable<TaskLabelDTO> taskLabelDTOs = taskLabels.Select(t => t.ToDTO());
            return taskLabelDTOs;
        }

        public Task<IEnumerable<TaskLabelDTO>> GetTaskLabelsByTaskItemIdAsync(int userId, int taskItemId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TaskLabelDTO>> GetTaskLabelsByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabel?> taskLabels = await _taskLabelRepository.GetByUserId(userId, cancellationToken);
            if (taskLabels == null || !taskLabels.Any())
                throw new NotFoundException("TaskLabel cannot be found");

            IEnumerable<TaskLabelDTO> taskLabelDTOs = taskLabels.Select(t => t.ToDTO());
            return taskLabelDTOs;
        }

        public async Task<int> CreateTaskLabelAsync(string name, int labelColor, int userId, int taskItemId, CancellationToken cancellationToken)
        {
            TaskLabel taskLabel = new(name, (LabelColor)labelColor, userId, taskItemId);
            await _taskLabelRepository.Create(taskLabel, cancellationToken);
            return taskLabel.Id;
        }

        public async Task<bool> UpdateTaskLabelAsync(int id, string name, int labelColor, int userId, int taskItemId, CancellationToken cancellationToken)
        {
            TaskLabel? taskLabel = await _taskLabelRepository.GetById(id, cancellationToken);
            if (taskLabel == null)
                throw new NotFoundException(nameof(taskLabel), id);

            taskLabel.Update(name, (LabelColor)labelColor, taskItemId);
            await _taskLabelRepository.Update(taskLabel, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteTaskLabelAsync(int id, CancellationToken cancellationToken)
        {
            TaskLabel? taskLabel = await _taskLabelRepository.GetById(id, cancellationToken);
            if (taskLabel == null)
                throw new NotFoundException(nameof(TaskLabel), id);

            taskLabel.MarkAsDeleted();
            await _taskLabelRepository.Update(taskLabel, cancellationToken);
            return true;
        }
    }
}
