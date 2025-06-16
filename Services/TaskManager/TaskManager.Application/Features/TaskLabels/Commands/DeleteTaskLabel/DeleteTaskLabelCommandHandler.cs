using BuildingBlocks.Exceptions;
using BuildingBlocks.CQRS.Request;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Commands.DeleteTaskLabel
{
    public class DeleteTaskLabelCommandHandler : IRequestHandler<DeleteTaskLabelCommand, DeleteTaskLabelResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public DeleteTaskLabelCommandHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<DeleteTaskLabelResult> Handle(DeleteTaskLabelCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException("Command cannot be null");

            var result = await _taskLabelService.DeleteAsync(command.Id, cancellationToken);
            return new DeleteTaskLabelResult(result);
        }
    }
}
