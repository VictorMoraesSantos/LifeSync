using BuildingBlocks.CQRS.Request;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Commands.CreateTaskLabel
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
            CreateTaskLabelDTO dto = new(
                command.Name,
                command.LabelColor,
                command.UserId,
                command.TaskItemId);

            var result = await _taskLabelService.CreateAsync(dto, cancellationToken);
        return new CreateTaskLabelResult(result);
        }
    }
}
