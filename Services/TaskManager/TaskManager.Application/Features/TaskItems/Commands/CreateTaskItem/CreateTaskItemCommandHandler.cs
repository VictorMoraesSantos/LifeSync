using MediatR;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.CreateTaskItem
{
    public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, CreateTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;

        public CreateTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<CreateTaskItemResult> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            CreateTaskItemDTO dto = new(
                command.Title,
                command.Description,
                command.Priority,
                command.DueDate,
                command.UserId);

            int result = await _taskItemService.CreateAsync(dto, cancellationToken);

            return new CreateTaskItemResult(result);
        }
    }
}
