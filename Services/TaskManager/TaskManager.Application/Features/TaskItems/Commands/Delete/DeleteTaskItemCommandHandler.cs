using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.DeleteTaskItem
{
    public class DeleteTaskItemCommandHandler : ICommandHandler<DeleteTaskItemCommand, DeleteTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;

        public DeleteTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<DeleteTaskItemResult>> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {

            Result<bool> result = await _taskItemService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteTaskItemResult>(result.Error!);

            return Result.Success(new DeleteTaskItemResult(result.Value!));
        }
    }
}
