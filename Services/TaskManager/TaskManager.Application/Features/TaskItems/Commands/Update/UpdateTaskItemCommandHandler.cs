using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.Update
{
    public class UpdateTaskItemCommandHandler : ICommandHandler<UpdateTaskItemCommand, UpdateTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;

        public UpdateTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<UpdateTaskItemResult>> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateTaskItemDTO(
                command.Id,
                command.Title,
                command.Description,
                command.Status,
                command.Priority,
                command.DueDate);

            var result = await _taskItemService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateTaskItemResult>(result.Error!);

            return Result.Success(new UpdateTaskItemResult(result.Value!));
        }
    }
}
