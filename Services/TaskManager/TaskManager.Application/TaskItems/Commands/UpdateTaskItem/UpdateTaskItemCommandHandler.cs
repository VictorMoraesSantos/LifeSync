using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Commands.UpdateTaskItem
{
    public class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand, bool>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public UpdateTaskItemCommandHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<bool> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException(nameof(command), "Command cannot be null");

            TaskItem? taskItem = await _taskItemRepository.GetById(command.Id, cancellationToken);
            if (taskItem == null)
                throw new NotFoundException(nameof(taskItem), command.Id);

            taskItem.Update(command.Title, command.Description, command.Priority, command.DueDate);

            await _taskItemRepository.Update(taskItem, cancellationToken);

            return true;
        }
    }
}
