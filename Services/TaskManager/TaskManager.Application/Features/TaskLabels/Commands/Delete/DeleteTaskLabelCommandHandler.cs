using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Commands.DeleteTaskLabel
{
    public class DeleteTaskLabelCommandHandler : ICommandHandler<DeleteTaskLabelCommand, DeleteTaskLabelResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public DeleteTaskLabelCommandHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<Result<DeleteTaskLabelResult>> Handle(DeleteTaskLabelCommand command, CancellationToken cancellationToken)
        {
            var result = await _taskLabelService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteTaskLabelResult>(result.Error!);

            return Result.Success(new DeleteTaskLabelResult(result.Value!));
        }
    }
}
