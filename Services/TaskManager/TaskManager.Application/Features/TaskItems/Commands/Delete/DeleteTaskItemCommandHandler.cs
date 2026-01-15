using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.Delete
{
    public class DeleteTaskItemCommandHandler : ICommandHandler<DeleteTaskItemCommand, DeleteTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;

        public DeleteTaskItemCommandHandler(
            ITaskItemService taskItemService,
            IHttpContextAccessor httpContext)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<DeleteTaskItemResult>> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {
            Result<TaskItemDTO?> existingTask = await _taskItemService.GetByIdAsync(command.Id, cancellationToken);
            if (!existingTask.IsSuccess)
                return Result.Failure<DeleteTaskItemResult>(existingTask.Error!);

            Result<bool> result = await _taskItemService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteTaskItemResult>(result.Error!);

            return Result.Success(new DeleteTaskItemResult(result.Value!));
        }
    }
}
