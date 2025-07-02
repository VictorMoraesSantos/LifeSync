using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Commands.CreateTaskLabel
{
    public class CreateTaskLabelCommandHandler : ICommandHandler<CreateTaskLabelCommand, CreateTaskLabelResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public CreateTaskLabelCommandHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<Result<CreateTaskLabelResult>> Handle(CreateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            CreateTaskLabelDTO dto = new(
                command.Name,
                command.LabelColor,
                command.UserId,
                command.TaskItemId);

            var result = await _taskLabelService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateTaskLabelResult>(result.Error!);

            return Result.Success(new CreateTaskLabelResult(result.Value!));
        }
    }
}
