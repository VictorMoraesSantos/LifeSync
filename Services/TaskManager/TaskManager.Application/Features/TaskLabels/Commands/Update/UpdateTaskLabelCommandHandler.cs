using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskLabels.Commands.Update
{
    public class UpdateTaskLabelCommandHandler : ICommandHandler<UpdateTaskLabelCommand, UpdateTaskLabelResult>
    {
        private readonly ITaskLabelService _taskLabelService;

        public UpdateTaskLabelCommandHandler(ITaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;
        }

        public async Task<Result<UpdateTaskLabelResult>> Handle(UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            UpdateTaskLabelDTO? dto = new(
                command.Id,
                command.Name,
                command.LabelColor);

            var result = await _taskLabelService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateTaskLabelResult>(result.Error!);

            return Result.Success(new UpdateTaskLabelResult(result.Value!));
        }
    }
}
