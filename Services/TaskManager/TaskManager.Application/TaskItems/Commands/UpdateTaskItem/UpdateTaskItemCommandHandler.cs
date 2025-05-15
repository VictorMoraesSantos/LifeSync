using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Commands.UpdateTaskItem
{
    public class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand, UpdateTaskItemCommandResult>
    {
        private readonly ITaskItemService _taskItemService;

        public UpdateTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<UpdateTaskItemCommandResult> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException(nameof(command), "Command cannot be null");

            bool result = await _taskItemService.UpdateTaskItemAsync(
                command.Id,
                command.Title,
                command.Description,
                (int)command.Status,
                (int)command.Priority,
                command.DueDate,
                cancellationToken);

            UpdateTaskItemCommandResult response = new(result);
            return response;
        }
    }
}
