using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskLabels.Commands.UpdateTaskLabel
{
    public class UpdateTaskLabelCommandHandler : IRequestHandler<UpdateTaskLabelCommand, UpdateTaskLabelResult>
    {
        private readonly ITaskLabelService _taskLabelService;
        
        public UpdateTaskLabelCommandHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }
        
        public async Task<UpdateTaskLabelResult> Handle(UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException("Command cannot be null");
        
            bool result = await _taskLabelService.UpdateTaskLabelAsync(command.Id, command.Name, (int)command.LabelColor, command.UserId, command.TaskItemId, cancellationToken);
            UpdateTaskLabelResult response = new(result);
            return response;
        }
    }   
}
