using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Commands.DeleteTaskLabel
{
    public class DeleteTaskLabelCommandHandler : IRequestHandler<DeleteTaskLabelCommand, DeleteTaskLabelResponse>
    {
        private readonly ITaskLabelService _taskLabelService;

        public DeleteTaskLabelCommandHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<DeleteTaskLabelResponse> Handle(DeleteTaskLabelCommand command, CancellationToken cancellationToken)
        {
            if(command == null)
                throw new BadRequestException("Command cannot be null");

            bool result = await _taskLabelService.DeleteTaskLabelAsync(command.Id, cancellationToken);
            DeleteTaskLabelResponse response = new(result);
            return response;
        }
    }
}
