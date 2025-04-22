using BuildingBlocks.Exceptions;
using System.Reflection.Emit;
using System.Threading;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Mapping;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TaskManager.Infrastructure.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public TaskItemService(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<TaskItemDTO> GetTaskItemByIdAsync(int id, CancellationToken cancellationToken)
        {
            TaskItem? taskItem = await _taskItemRepository.GetById(id, cancellationToken);
            if (taskItem == null)
                throw new NotFoundException(nameof(taskItem), id);

            TaskItemDTO taskItemDTIO = taskItem.ToDTO();
            return taskItemDTIO;
        }

        public async Task<IEnumerable<TaskItemDTO?>> GetTaskItemsByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetByUserId(userId, cancellationToken);
            if (taskItems == null)
                throw new NotFoundException(nameof(taskItems), userId);

            IEnumerable<TaskItemDTO> taskItemsDTO = taskItems.Select(TaskItemMapper.ToDTO);
            return taskItemsDTO;
        }

        public async Task<IEnumerable<TaskItemDTO>> GetTaskItemsByDueDateAsync(int userId, DateOnly dueDate, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetByDueDate(userId, dueDate, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);
            return taskItemDTOs;
        }

        public async Task<IEnumerable<TaskItemDTO>> GetTaskItemsByLabelIdAsync(int userId, int labelId, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetByLabel(userId, labelId, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);
            return taskItemDTOs;
        }

        public async Task<IEnumerable<TaskItemDTO>> GetTaskItemsByPriorityAsync(int userId, Priority priority, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetByPriority(userId, priority, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);
            return taskItemDTOs;
        }

        public async Task<IEnumerable<TaskItemDTO>> GetTaskItemsByStatusAsync(int userId, Status status, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetByStatus(userId, status, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);
            return taskItemDTOs;
        }

        public async Task<IEnumerable<TaskItemDTO>> GetTaskItemsTitleAsync(int userId, string title, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetTitleContains(userId, title, cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);
            return taskItemDTOs;
        }


        public async Task<IEnumerable<TaskItemDTO>> GetAllTaskItemsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<TaskItem?> taskItems = await _taskItemRepository.GetAll(cancellationToken);
            if (taskItems == null || !taskItems.Any())
                throw new NotFoundException("TaskItems was not found");

            IEnumerable<TaskItemDTO> taskItemDTOs = taskItems.Select(TaskItemMapper.ToDTO);
            return taskItemDTOs;
        }

        public async Task<bool> UpdateTaskItemAsync(int id, string title, string description, int status, int priority, DateOnly dueDate, CancellationToken cancellationToken)
        {
            TaskItem? taskItem = await _taskItemRepository.GetById(id, cancellationToken);
            if (taskItem == null)
                throw new NotFoundException(nameof(taskItem), id);

            taskItem.Update(title, description, (Status)status, (Priority)priority, dueDate);
            await _taskItemRepository.Update(taskItem, cancellationToken);
            return true;
        }

        public async Task<int> CreateTaskItemAsync(string title, string description, int priority, DateOnly dueDate, int userId, CancellationToken cancellationToken)
        {
            TaskItem taskItem = new(title, description, (Priority)priority, dueDate, userId);
            await _taskItemRepository.Create(taskItem, cancellationToken);
            return taskItem.Id;
        }

        public async Task<bool> DeleteTaskItemAsync(int id, CancellationToken cancellationToken)
        {
            TaskItem? taskItem = await _taskItemRepository.GetById(id, cancellationToken);
            if (taskItem == null)
                throw new NotFoundException(nameof(taskItem), id);

            taskItem.MarkAsDeleted();
            await _taskItemRepository.Update(taskItem, cancellationToken);
            return true;
        }
    }
}
