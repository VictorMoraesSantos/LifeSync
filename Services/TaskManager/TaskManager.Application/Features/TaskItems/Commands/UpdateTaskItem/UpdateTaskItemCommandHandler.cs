using MediatR;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.UpdateTaskItem
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
            UpdateTaskItemDTO dto = new(
                command.Id,
                command.Title,
                command.Description,
                command.Status,
                command.Priority,
                command.DueDate);

            bool result = await _taskItemService.UpdateAsync(dto, cancellationToken);

            UpdateTaskItemCommandResult response = new(result);
            return response;
        }
    }
}
