using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
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
            UpdateTaskLabelDTO? dto = new(
                command.Id,
                command.Name,
                command.LabelColor);

            bool result = await _taskLabelService.UpdateAsync(dto, cancellationToken);
            UpdateTaskLabelResult response = new(result);
            return response;
        }
    }
}
