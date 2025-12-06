using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.Features.TaskItems.Commands.Create;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Features.TaskItems.Commands.Update
{
    public class UpdateTaskItemCommandHandler :
        SecureCommandHandlerBase,
        ICommandHandler<UpdateTaskItemCommand, UpdateTaskItemResult>
    {
        private readonly ITaskItemService _taskItemService;
        private readonly IValidator<UpdateTaskItemCommand> _validator;

        public UpdateTaskItemCommandHandler(
            ITaskItemService taskItemService,
            IValidator<UpdateTaskItemCommand> validator,
            IHttpContextAccessor httpContext) : base(httpContext)
        {
            _taskItemService = taskItemService;
        }

        public async Task<Result<UpdateTaskItemResult>> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            var existingTask = await _taskItemService.GetByIdAsync(command.Id, cancellationToken);
            if (!existingTask.IsSuccess)
                return Result.Failure<UpdateTaskItemResult>(existingTask.Error!);

            var accessValidation = ValidateAccess<UpdateTaskItemResult>(existingTask.Value!.UserId);
            if (!accessValidation.IsSuccess)
                return accessValidation;

            ValidationResult validation = await _validator.ValidateAsync(command, cancellationToken);
            if (!validation.IsValid)
            {
                List<string> errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Failure<UpdateTaskItemResult>(new Error(string.Join("; ", errors), ErrorType.Validation));
            }

            var dto = new UpdateTaskItemDTO(
                command.Id,
                command.Title,
                command.Description,
                command.Status,
                command.Priority,
                command.DueDate);

            var result = await _taskItemService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateTaskItemResult>(result.Error!);

            return Result.Success(new UpdateTaskItemResult(result.Value!));
        }
    }
}
