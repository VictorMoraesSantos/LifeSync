using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.AddLabel
{
    public class AddLabelCommandHandler : SecureCommandHandlerBase, ICommandHandler<AddLabelCommand, AddLabelResult>
    {
        private readonly ITaskItemService _taskItemService;
        private readonly IValidator<AddLabelCommand> _validator;

        public AddLabelCommandHandler(
            ITaskItemService taskItemService,
            IValidator<AddLabelCommand> validator,
            IHttpContextAccessor httpContext)
            : base(httpContext)
        {
            _taskItemService = taskItemService;
            _validator = validator;
        }

        public async Task<Result<AddLabelResult>> Handle(AddLabelCommand command, CancellationToken cancellationToken)
        {
            ValidationResult validation = await _validator.ValidateAsync(command, cancellationToken);
            if (!validation.IsValid)
            {
                List<string> errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Failure<AddLabelResult>(new Error(string.Join("; ", errors), ErrorType.Validation));
            }

            Result<TaskItemDTO?> existingTask = await _taskItemService.GetByIdAsync(command.TaskItemId, cancellationToken);
            if (!existingTask.IsSuccess)
                return Result.Failure<AddLabelResult>(existingTask.Error!);

            //Result<AddLabelResult> accessValidation = ValidateAccess<AddLabelResult>(existingTask.Value!.UserId);
            //if (!accessValidation.IsSuccess)
            //    return Result.Failure<AddLabelResult>(accessValidation.Error!);

            var dto = new UpdateLabelsDTO(command.TaskItemId, command.TaskLabelsId);

            Result<bool> result = await _taskItemService.AddLabelAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<AddLabelResult>(result.Error!);

            return Result.Success<AddLabelResult>(new AddLabelResult(result.Value));
        }
    }
}
