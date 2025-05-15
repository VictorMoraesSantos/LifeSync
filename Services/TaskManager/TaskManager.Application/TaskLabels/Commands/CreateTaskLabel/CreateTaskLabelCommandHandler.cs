using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Commands.CreateTaskLabel
{
    public class CreateTaskLabelCommandHandler : IRequestHandler<CreateTaskLabelCommand, CreateTaskLabelResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public CreateTaskLabelCommandHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<CreateTaskLabelResult> Handle(CreateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException("Command cannot be null");

            int result = await _taskLabelService.CreateTaskLabelAsync(command.Name, (int)command.LabelColor, command.UserId, command.TaskItemId, cancellationToken);
            CreateTaskLabelResult response = new(result);
            return response;
        }
    }
}
