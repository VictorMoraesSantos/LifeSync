using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.DeleteTaskItem
{
    public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, Result<DeleteTaskItemResult>>
    {
        private readonly ITaskItemService _taskItemService;

        public DeleteTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<DeleteTaskItemResult>> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {

            var result = await _taskItemService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteTaskItemResult>(result.Error!);

            return Result.Success(new DeleteTaskItemResult(result.Value!));
        }
    }
}
