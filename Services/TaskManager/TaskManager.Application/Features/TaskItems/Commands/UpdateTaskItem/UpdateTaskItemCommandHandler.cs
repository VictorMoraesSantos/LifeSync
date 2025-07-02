using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.UpdateTaskItem
{
    public class UpdateTaskItemCommandHandler : ICommandHandler<UpdateTaskItemCommand, UpdateTaskItemCommandResult>
    {
        private readonly ITaskItemService _taskItemService;

        public UpdateTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<UpdateTaskItemCommandResult>> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            UpdateTaskItemDTO dto = new(
                command.Id,
                command.Title,
                command.Description,
                command.Status,
                command.Priority,
                command.DueDate);

            var result = await _taskItemService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateTaskItemCommandResult>(result.Error!);

            return Result.Success(new UpdateTaskItemCommandResult(result.Value!));
        }
    }
}
