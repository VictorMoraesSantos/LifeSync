using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.Update
{
    public class UpdateTaskItemCommandHandler : ICommandHandler<UpdateTaskItemCommand, UpdateTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;
        private readonly IValidator<UpdateTaskItemCommand> _validator;

        public UpdateTaskItemCommandHandler(
            ITaskItemService taskItemService,
            IValidator<UpdateTaskItemCommand> validator,
            IHttpContextAccessor httpContext)
        {
            _taskItemService = taskItemService;
            _validator = validator;
        }

        public async Task<Result<UpdateTaskItemResult>> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            ValidationResult validation = await _validator.ValidateAsync(command, cancellationToken);
            if (!validation.IsValid)
            {
                List<string> errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Failure<UpdateTaskItemResult>(new Error(string.Join("; ", errors), ErrorType.Validation));
            }

            Result<TaskItemDTO?> existingTask = await _taskItemService.GetByIdAsync(command.Id, cancellationToken);
            if (!existingTask.IsSuccess)
                return Result.Failure<UpdateTaskItemResult>(existingTask.Error!);

            UpdateTaskItemDTO dto = new(
                command.Id,
                command.Title,
                command.Description,
                command.Status,
                command.Priority,
                command.DueDate);

            Result<bool> result = await _taskItemService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateTaskItemResult>(result.Error!);

            return Result.Success(new UpdateTaskItemResult(result.Value!));
        }
    }
}
