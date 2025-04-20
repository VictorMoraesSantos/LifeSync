using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Commands.CreateTaskItem
{
    public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, int>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public CreateTaskItemCommandHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<int> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException(nameof(command), "Command cannot be null");

            TaskItem taskItem = new(command.Title, command.Description, command.Priority, command.DueDate, command.UserId);
            await _taskItemRepository.Create(taskItem, cancellationToken);

            return taskItem.Id;
        }
    }
}
