using BuildingBlocks.CQRS.Request;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.DeleteTaskItem
{
    public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, DeleteTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;

        public DeleteTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<DeleteTaskItemResult> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {

            var result = await _taskItemService.DeleteAsync(command.Id, cancellationToken);
            return new DeleteTaskItemResult(result);
        }
    }
}
